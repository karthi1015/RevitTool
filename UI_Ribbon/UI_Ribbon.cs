using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autodesk.Revit.UI;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Net;
using UI_Ribbon.Data;
using UI_Ribbon.Data.Binding;
using Newtonsoft.Json;

namespace UI_Ribbon
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    class App : IExternalApplication
    {
        static string AddinPath = typeof(App).Assembly.Location;
        static string ButtonIconsFolder = Path.GetDirectoryName(AddinPath);

        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                string addin = Check_Permission(application);

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Ribbon", ex.ToString());
                return Result.Failed;
            }
        }

        string pathUserPassword = @"C:\Users\" + Environment.UserName + "\\AppData\\Roaming\\Pentacons\\User";
        string pathListAddIn3 = @"\\192.168.1.250\data\DataBases\09 DataAddIn\ListAddIn_Revit.txt";
        List<string> AddIn_Add = new List<string>();

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public string Check_Permission(UIControlledApplication application)
        {
            string addin = null;
            try
            {
                List<data_information> data = new List<data_information>();
                var listDataUser = SQL.SQLRead(Source.path_WEB, "dbo.spRead_User", Source.type_Query, new List<string>(), new List<string>());
                for (var i = 0; i < listDataUser.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(listDataUser.Rows[i]["SystemRole"].ToString()))
                    {
                        data.Add(new data_information()
                        {
                            user_name = listDataUser.Rows[i]["UserName"].ToString(),
                            user_id = listDataUser.Rows[i]["UserId"].ToString(),
                            user_password = listDataUser.Rows[i]["UserPassWord"].ToString(),
                            role = listDataUser.Rows[i]["SystemRole"].ToString()
                        });
                    }
                }

                List<data_role> role = new List<data_role>();
                var listDataRole = SQL.SQLRead(Source.path_Manage, "dbo.sp_ReadData_RoleManage", Source.type_Query, new List<string>(), new List<string>());
                for (var i = 0; i < listDataRole.Rows.Count; i++)
                {
                    role.Add(new data_role()
                    {
                        role = listDataRole.Rows[i]["Role"].ToString(),
                        addin = listDataRole.Rows[i]["Addin"].ToString()
                    });
                }

                string myIP = Dns.GetHostAddresses(Dns.GetHostName()).First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
                string path = pathUserPassword + "\\" + myIP;

                if (File.Exists(path))
                {
                    string role_addin = "";
                    data_information data_infor = JsonConvert.DeserializeObject<data_information>(File.ReadAllText(path));
                    var infor = data.Where(x => x.user_id == data_infor.user_id && x.user_password == data_infor.user_password).ToList();
                    if (infor.Count() == 1)
                    {
                        role_addin = data_infor.role;
                    }

                    var data_addin = role.Where(x => x.role == role_addin).ToList();
                    if(data_addin.Count() == 1)
                    {
                        addin = data_addin[0].addin;
                    }
                    if (string.IsNullOrEmpty(addin))
                    {
                        MessageBoxResult messageBoxResult = MessageBox.Show("You are not logged in !!! \nAre you logged in ???", "ERROR", MessageBoxButton.YesNo, MessageBoxImage.Error);
                        if (messageBoxResult == MessageBoxResult.Yes)
                        {
                            UserControl1 mainWindow = new UserControl1();
                            mainWindow.ShowDialog();
                            Check_Permission(application);
                        }
                    }
                    else
                    {
                        AddRibbonPanel(application, addin);
                    }
                }
            }
            catch (Exception)
            {
                if (string.IsNullOrEmpty(addin))
                {
                    MessageBoxResult messageBoxResult = MessageBox.Show("You are not logged in !!! \nAre you logged in ???", "ERROR", MessageBoxButton.YesNo, MessageBoxImage.Error);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        UserControl1 mainWindow = new UserControl1();
                        mainWindow.ShowDialog();
                        Check_Permission(application);
                    }
                }
            }
            return addin;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //Create a push button to trigger a command add it to the ribbon panel.
        string tabName = "Pentacons";
        string pathDll = @"\\192.168.1.250\data\DataBases\01 RevitDataBases\04 Add_in\01 Revit_API\DLL\";
        string pathImage = @"\\192.168.1.250\data\DataBases\01 RevitDataBases\04 Add_in\01 Revit_API\Resources\Image\";

        //string tabName = "W";
        //string pathDll = @"D:\00 CuaKhanh\03 Revit\00 Ribbon_Mr.W\DLL\";
        //string pathImage = @"D:\00 CuaKhanh\03 Revit\00 Ribbon_Mr.W\Image\";
        public void AddRibbonPanel(UIControlledApplication application, string addin)
        {
            //add danh sách addin
            List<string> lines3 = File.ReadAllLines(pathListAddIn3).ToList();
            foreach (string line in lines3)
            {
                AddIn_Add.Add(line);
            }

            application.CreateRibbonTab(tabName);

            RibbonPanel ribbonSet = application.CreateRibbonPanel(tabName, "Setup Tool");
            RibbonPanel ribbonQua = application.CreateRibbonPanel(tabName, "Quantity");
            RibbonPanel ribbonArc = application.CreateRibbonPanel(tabName, "Support Achitecture");
            RibbonPanel ribbonFo = application.CreateRibbonPanel(tabName, "Support FormWork");
            RibbonPanel ribbonAll = application.CreateRibbonPanel(tabName, "Support Allplan");
            RibbonPanel ribbonNav = application.CreateRibbonPanel(tabName, "Support Naviswork");
            RibbonPanel ribbonWeb = application.CreateRibbonPanel(tabName, "Support Web");
            RibbonPanel ribbonTool = application.CreateRibbonPanel(tabName, "Support Tool");
            RibbonPanel ribbonDraw = application.CreateRibbonPanel(tabName, "Draw Tool");

            ////Phân quyền sử dụng add-in-----------------------------------------------------------------------------------------------------------------------------------------------------------
            if (addin != null)
            {
                try
                {
                    if (addin.Contains(AddIn_Add[0]))
                    {
                        //Filter Tool
                        PushButton pushButton = ribbonTool.AddItem(create_pushdata(new List<string>() { "Tool_Filter\\Tool_Filter.dll", "Multiple" + "\r\n" + "Filter", "Tool_Filter.Tool_Filter", "Multifilter.png", "Super filter" })) as PushButton;
                    }
                }
                catch (Exception) { }

                try
                {
                    if (addin.Contains(AddIn_Add[1]))
                    {
                        //Filter Tool
                        PushButton pushButton = ribbonSet.AddItem(create_pushdata(new List<string>() { "SetupTool_MaterialSetup\\SetupTool_MaterialSetup.dll", "Material" + "\r\n" + "Mangement", "SetupTool_MaterialSetup.SetupTool_MaterialSetup", "MaterialManager.png", "Tạo và quản lý vật liệu" })) as PushButton;
                    }
                }
                catch (Exception) { }

                try
                {
                    if (addin.Contains(AddIn_Add[2]))
                    {
                        //Filter Tool
                        PushButton pushButton = ribbonQua.AddItem(create_pushdata(new List<string>() { "ARC_Quantity\\ARC_Quatity.dll", "All" + "\r\n" + "Quantity", "ARC_Quatity.ARC_Quatity", "MaterialTakeoff.png", "Lấy và upload khối lượng vào dữ liệu tổng" })) as PushButton;
                    }
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }

                try
                {
                    ribbonSet.AddSeparator();
                    if (addin.Contains(AddIn_Add[3]))
                    {
                        //Filter Tool
                        PushButton pushButton = ribbonSet.AddItem(create_pushdata(new List<string>() { "SetupTool_TypeSetup\\SetupTool_TypeSetup.dll", "Manage" + "\r\n" + "ElementType", "SetupTool_TypeSetup.SetupTool_TypeSetup", "ElementMa.png", "Tạo và quản lý type" })) as PushButton;
                    }
                }
                catch (Exception) { }

                try
                {
                    ribbonTool.AddSeparator();
                    if (addin.Contains(AddIn_Add[4]))
                    {
                        //Filter Tool
                        PushButton pushButton = ribbonTool.AddItem(create_pushdata(new List<string>() { "LevelElevation.dll", "Level" + "\r\n" + "Control", "LevelElevation.Command", "UpdateLevel.png", "Tạo, quản lý và cập nhật level của dự án" })) as PushButton;
                    }
                }
                catch (Exception) { }

                try
                {
                    if (addin.Contains(AddIn_Add[5]))
                    {
                        //Filter Tool
                        PushButton pushButton = ribbonNav.AddItem(create_pushdata(new List<string>() { "Naviswork_ClashComment\\Naviswork_ClashComment.dll", "Check Clash", "Naviswork_ClashComment.Naviswork_ClashComment", "ClashCheck.png", "Kiểm tra và cập nhật thông tin chỉnh sửa lên dữ liệu tổng" })) as PushButton;
                    }
                }
                catch (Exception) { }

                try
                {
                    ribbonSet.AddSeparator();
                    if (addin.Contains(AddIn_Add[6]))
                    {
                        //Filter Tool
                        PushButton pushButton = ribbonSet.AddItem(create_pushdata(new List<string>() { "SetupTool_ParameterSetup\\SetupTool_ParameterSetup.dll", "Setup" + "\r\n" + "Parameters", "SetupTool_ParameterSetup.SetupTool_ParameterSetup", "ParamterSetting.png", "Thêm, cập nhật thông tin và parameter cần thiết cho dự án" })) as PushButton;
                    }
                }
                catch (Exception) { }

                try
                {
                    if (addin.Contains(AddIn_Add[7]))
                    {
                        //Filter Tool
                        PushButton pushButton = ribbonWeb.AddItem(create_pushdata(new List<string>() { "WEB_SaveAs\\WEB_SaveAs.dll", "Save As", "WEB_SaveAs.WEB_SaveAs", "SaveAs.png", "Chia nhỏ mô hình và loại bỏ những thông tin không cần thiết trên dữ liệu WEB" })) as PushButton;
                    }
                }
                catch (Exception) { }

                try
                {
                    if (addin.Contains(AddIn_Add[8]))
                    {
                        //Filter Tool
                        PushButton pushButton = ribbonAll.AddItem(create_pushdata(new List<string>() { "Allplan_ParameterSupport\\Allplan_ParameterSupport.dll", "Allplan" + "\r\n" + "Parameters", "Allplan_ParameterSupport.Allplan_ParameterSupport", "addUniqueId.png", "Thêm, cập nhật thông tin cho mô hình trên liên kết trên Allplan và WEB" })) as PushButton;
                    }
                }
                catch (Exception) { }

                try
                {
                    ribbonQua.AddSeparator();
                    if (addin.Contains(AddIn_Add[9]))
                    {
                        //Filter Tool
                        PushButton pushButton = ribbonQua.AddItem(create_pushdata(new List<string>() { "Tool_ViewInformation\\Tool_ViewInformation.dll", "View" + "\r\n" + "Quantity", "Tool_ViewInformation.Tool_ViewInformation", "InquiryID.png", "Xem thông tin khối lượng của cấu kiện được chọn" })) as PushButton;
                    }
                }
                catch (Exception) { }

                try
                {
                    ribbonTool.AddSeparator();
                    if (addin.Contains(AddIn_Add[10]))
                    {
                        //Filter Tool
                        PushButton pushButton = ribbonTool.AddItem(create_pushdata(new List<string>() { "ExportTool_GetLocation\\GetCoordinatesElement.dll", "Get" + "\r\n" + "Location", "GetCoordinatesElement.Command", "getLocation.png", "Lấy và cập nhật thông tin vị trí so với lưới trục của cấu kiện" })) as PushButton;
                    }
                }
                catch (Exception) { }

                try
                {
                    ribbonTool.AddSeparator();
                    if (addin.Contains(AddIn_Add[11]))
                    {
                        //Filter Tool
                        PushButton pushButton = ribbonTool.AddItem(create_pushdata(new List<string>() { "Tool_AssignElementId\\ExportDataToSQL.dll", "Assign" + "\r\n" + "ElementId", "creatData.Command", "assignElementId.png", "Thêm, Cập nhật cấu kiện liên quan với bản vẽ" })) as PushButton;
                    }
                }
                catch (Exception) { }

                try
                {
                    if (addin.Contains(AddIn_Add[12]))
                    {
                        //Filter Tool
                        PushButton pushButton = ribbonFo.AddItem(create_pushdata(new List<string>() { "Formwork_OpenningElevation\\filloutUKRD.dll", "Opening" + "\r\n" + "Elevation", "filloutUKRD.Command", "opening_elevation.png", "Cập nhật thông tin cao độ bản vẽ formwork" })) as PushButton;
                    }
                }
                catch (Exception) { }
                try
                {
                    if (addin.Contains(AddIn_Add[13]))
                    {
                        //Filter Tool
                        //SplitButtonData sb1 = new SplitButtonData("splitButton1", "Split");
                        //SplitButton sb = ribbonArc.AddItem(sb1) as SplitButton;
                        PulldownButton pulldownButton = ribbonArc.AddItem(add_items(new List<string>() { "Support" + " " + "Layout", "Achitecture.ico", "Cập nhật thông tin hỗ trợ layout kiến trúc" })) as PulldownButton;
                        PushButton pushButton = pulldownButton.AddPushButton(create_pushdata(new List<string>() { "ARC_SupportLayout\\ARC_SupportLayout.dll", "Width" + " " + "Finish", "ARC_SupportLayout.ARC_SupportLayout", "draw.ico", "Cập nhật thông tin hỗ trợ layout kiến trúc" })) as PushButton;
                    }
                }
                catch (Exception) { }
            }

            ribbonTool.AddSeparator();
            PushButton pushButtonPrint = ribbonTool.AddItem(create_pushdata(new List<string>() { "Tool_AutoPrint\\Auto_Prin_PDF.dll", "Super Print", "Print.Command", "printer.png", "In PDF, DWG theo setup sẵn" })) as PushButton;

            PushButton pushButtonDraw = ribbonDraw.AddItem(create_pushdata(new List<string>() { "Draw_All\\Draw_All.dll", "Super Draw", "Draw_All.Draw_All", "DrawAll.ico", "Dựng mô hình, phục vụ công tác layout formwork" })) as PushButton;
        }

        public PushButtonData create_pushdata(List<string> infor_addin) //name_file_dll | name_in_UI | namepace.class | name_image | tooltip
        {
            PushButtonData push = null;
            try
            {
                // define 3 new buttons to be added as stacked buttons
                string GetQuantityDllPath = pathDll + infor_addin[0];
                push = new PushButtonData(infor_addin[1], infor_addin[1], GetQuantityDllPath, infor_addin[2]);
                Uri imageSourseGetQuantity = new Uri(pathImage + infor_addin[3]);
                push.LargeImage = new BitmapImage(imageSourseGetQuantity);
                push.ToolTip = infor_addin[4];
            }
            catch (Exception)
            {

            }
            return push;
        }

        public PushButtonData create_pushdata_pull(List<string> infor_addin) //name_file_dll | name_in_UI | namepace.class | name_image | tooltip
        {
            PushButtonData push = null;
            try
            {
                // define 3 new buttons to be added as stacked buttons
                string GetQuantityDllPath = pathDll + infor_addin[0];
                push = new PushButtonData(infor_addin[1], infor_addin[1], GetQuantityDllPath, infor_addin[2]);
                Uri imageSourseGetQuantity = new Uri(pathImage + infor_addin[3]);
                BitmapImage bitmapImage = new BitmapImage(imageSourseGetQuantity);
                push.LargeImage = bitmapImage;
                push.ToolTip = infor_addin[4];
            }
            catch (Exception)
            {

            }
            return push;
        }

        public PulldownButtonData add_items(List<string> infor_addin) //name_in_UI | name_image | tooltip
        {
            PulldownButtonData pull_down = null;
            try
            {
                // add a pull-down button to the panel 
                pull_down = new PulldownButtonData(infor_addin[0], infor_addin[0]);
                Uri imageSourseGetQuantity = new Uri(pathImage + infor_addin[1]);
                pull_down.LargeImage = new BitmapImage(imageSourseGetQuantity);
                pull_down.ToolTip = infor_addin[2];
            }
            catch (Exception)
            {

            }
            return pull_down;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }




    }
}