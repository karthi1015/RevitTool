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

namespace UI_Ribbon
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.Attributes.Regeneration(Autodesk.Revit.Attributes.RegenerationOption.Manual)]
    [Autodesk.Revit.Attributes.Journaling(Autodesk.Revit.Attributes.JournalingMode.NoCommandData)]
    class App : IExternalApplication
    {
        static string AddinPath = typeof(App).Assembly.Location;
        static string ButtonIconsFolder = Path.GetDirectoryName(AddinPath);
        FunctionSQL mySQL;
        FunctionSupoort myFunctionSupport;
        ListSource mySource;

        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                mySQL = new FunctionSQL();
                myFunctionSupport = new FunctionSupoort();
                mySource = new ListSource();

                var listtotal = mySQL.SQLRead(@"Server=18.141.116.111,1433\SQLEXPRESS;Database=ManageDataBase;User Id=ManageUser; Password = manage@connect789", "Select * from dbo.PathSource", "Query", new List<string>(), new List<string>());
                path = listtotal.Rows[0][1].ToString();
                Function_TXT();

                mySQL = new FunctionSQL();
                string addin = Check_Permission(application);
                
                return Result.Succeeded;
            }
            catch(Exception ex)
            {
                TaskDialog.Show("Ribbon", ex.ToString());
                return Result.Failed;
            }
        }

        //----------------------------------------------------------
        public string path = "";
        public All_Data myAll_Data { get; set; }
        public void Function_TXT()
        {
            try
            {
                myAll_Data = myFunctionSupport.Get_Data_All(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                List<string> Role = new List<string>();
                List<string> AddIn = new List<string>();

                List<string> UserId = new List<string>();
                List<string> UserName = new List<string>();
                List<string> Password = new List<string>();
                List<string> SystemRole = new List<string>();
                var listDataUser = mySQL.SQLRead(myAll_Data.list_path_connect_SQL_data[1], "dbo.spRead_User", mySource.type_Query, new List<string>(), new List<string>());
                for (var i = 0; i < listDataUser.Rows.Count; i++)
                {
                    if (!string.IsNullOrEmpty(listDataUser.Rows[i]["SystemRole"].ToString()))
                    {
                        UserId.Add(listDataUser.Rows[i]["UserId"].ToString());
                        UserName.Add(listDataUser.Rows[i]["UserName"].ToString());
                        Password.Add(listDataUser.Rows[i]["UserPassWord"].ToString());
                        SystemRole.Add(listDataUser.Rows[i]["SystemRole"].ToString());
                    }
                }

                var listDataRole = mySQL.SQLRead(myAll_Data.list_path_connect_SQL_data[2], "dbo.sp_ReadData_RoleManage", mySource.type_Query, new List<string>(), new List<string>());
                for (var i = 0; i < listDataRole.Rows.Count; i++)
                {
                    Role.Add(listDataRole.Rows[i]["Role"].ToString());
                    AddIn.Add(listDataRole.Rows[i]["Addin"].ToString());
                }

                string role = null;
                if (UserName.Count() > 0 && Password.Count() > 0 && SystemRole.Count() > 0 && Role.Count() > 0 && AddIn.Count() > 0 && UserId.Count() > 0)
                {
                    string userId = "";
                    string password = "";
                    string hostName = Dns.GetHostName();
                    string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
                    string path = pathUserPassword + "\\" + myIP;

                    if (File.Exists(path))
                    {
                        var infor = File.ReadAllLines(path).ToList();
                        userId = infor[1];
                        password = infor[2];
                    }
                    for (var i = 0; i < UserName.Count(); i++)
                    {
                        if (UserId[i] == userId && password == Password[i])
                        {
                            role = SystemRole[i];
                        }
                    }

                    for (var i = 0; i < Role.Count(); i++)
                    {
                        if (role != null && Role[i] == role)
                        {
                            addin = AddIn[i];
                        }
                    }
                    AddRibbonPanel(application, addin);
                }
            }
            catch (Exception )
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
            RibbonPanel ribbonFo = application.CreateRibbonPanel(tabName, "FormWork");
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
                        PushButton pushButton = ribbonTool.AddItem(create_pushdata(new List<string>() { "Tool_Filter.dll", "Multiple" + "\r\n" + "Filter", "Tool_Filter.Tool_Filter", "Multifilter.png", "Super filter" })) as PushButton;
                    }
                }
                catch (Exception){}

                try
                {
                    if (addin.Contains(AddIn_Add[1]))
                    {
                        //Filter Tool
                        PushButton pushButton = ribbonSet.AddItem(create_pushdata(new List<string>() { "SetupTool_MaterialSetup.dll", "Material" + "\r\n" + "Mangement", "SetupTool_MaterialSetup.SetupTool_MaterialSetup", "MaterialManager.png", "Tạo và quản lý vật liệu" })) as PushButton;
                    }
                }
                catch (Exception) { }

                try
                {
                    if (addin.Contains(AddIn_Add[2]))
                    {
                        //Filter Tool
                        PushButton pushButton = ribbonQua.AddItem(create_pushdata(new List<string>() { "ARC_Quatity.dll", "All" + "\r\n" + "Quantity", "ARC_Quatity.ARC_Quatity", "MaterialTakeoff.png", "Lấy và upload khối lượng vào dữ liệu tổng" })) as PushButton;
                    }
                }
                catch (Exception) { }

                try
                {
                    ribbonSet.AddSeparator();
                    if (addin.Contains(AddIn_Add[3]))
                    {
                        //Filter Tool
                        PushButton pushButton = ribbonSet.AddItem(create_pushdata(new List<string>() { "SetupTool_TypeSetup.dll", "Manage" + "\r\n" + "ElementType", "SetupTool_TypeSetup.SetupTool_TypeSetup", "ElementMa.png", "Tạo và quản lý type" })) as PushButton;
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
                        PushButton pushButton = ribbonNav.AddItem(create_pushdata(new List<string>() { "Naviswork_ClashComment.dll", "Check Clash", "Naviswork_ClashComment.Naviswork_ClashComment", "ClashCheck.png", "Kiểm tra và cập nhật thông tin chỉnh sửa lên dữ liệu tổng" })) as PushButton;
                    }
                }
                catch (Exception) { }

                try
                {
                    ribbonSet.AddSeparator();
                    if (addin.Contains(AddIn_Add[6]))
                    {
                        //Filter Tool
                        PushButton pushButton = ribbonSet.AddItem(create_pushdata(new List<string>() { "SetupTool_ParameterSetup.dll", "Setup" + "\r\n" + "Parameters", "SetupTool_ParameterSetup.SetupTool_ParameterSetup", "ParamterSetting.png", "Thêm, cập nhật thông tin và parameter cần thiết cho dự án" })) as PushButton;
                    }
                }
                catch (Exception) { }

                try
                {
                    if (addin.Contains(AddIn_Add[7]))
                    {
                        //Filter Tool
                        PushButton pushButton = ribbonWeb.AddItem(create_pushdata(new List<string>() { "WEB_SaveAs.dll", "Save As", "WEB_SaveAs.WEB_SaveAs", "SaveAs.png", "Chia nhỏ mô hình và loại bỏ những thông tin không cần thiết trên dữ liệu WEB" })) as PushButton;
                    }
                }
                catch (Exception) { }

                try
                {
                    if (addin.Contains(AddIn_Add[8]))
                    {
                        //Filter Tool
                        PushButton pushButton = ribbonAll.AddItem(create_pushdata(new List<string>() { "Allplan_ParameterSupport.dll", "Allplan" + "\r\n" + "Parameters", "Allplan_ParameterSupport.Allplan_ParameterSupport", "addUniqueId.png", "Thêm, cập nhật thông tin cho mô hình trên liên kết trên Allplan và WEB" })) as PushButton;
                    }
                }
                catch (Exception) { }

                try
                {
                    ribbonQua.AddSeparator();
                    if (addin.Contains(AddIn_Add[9]))
                    {
                        //Filter Tool
                        PushButton pushButton = ribbonQua.AddItem(create_pushdata(new List<string>() { "Tool_ViewInformation.dll", "View" + "\r\n" + "Quantity", "Tool_ViewInformation.Tool_ViewInformation", "InquiryID.png", "Xem thông tin khối lượng của cấu kiện được chọn" })) as PushButton;
                    }
                }
                catch (Exception) { }

                try
                {
                    ribbonTool.AddSeparator();
                    if (addin.Contains(AddIn_Add[10]))
                    {
                        //Filter Tool
                        PushButton pushButton = ribbonTool.AddItem(create_pushdata(new List<string>() { "GetCoordinatesElement.dll", "Get" + "\r\n" + "Location", "GetCoordinatesElement.Command", "getLocation.png", "Lấy và cập nhật thông tin vị trí so với lưới trục của cấu kiện" })) as PushButton;
                    }
                }
                catch (Exception) { }

                try
                {
                    ribbonTool.AddSeparator();
                    if (addin.Contains(AddIn_Add[11]))
                    {
                        //Filter Tool
                        PushButton pushButton = ribbonTool.AddItem(create_pushdata(new List<string>() { "ExportDataToSQL.dll", "Assign" + "\r\n" + "ElementId", "creatData.Command", "assignElementId.png", "Thêm, Cập nhật cấu kiện liên quan với bản vẽ" })) as PushButton;
                    }
                }
                catch (Exception) { }

                try
                {
                    if (addin.Contains(AddIn_Add[12]))
                    {
                        //Filter Tool
                        PushButton pushButton = ribbonFo.AddItem(create_pushdata(new List<string>() { "filloutUKRD.dll", "Opening" + "\r\n" + "Elevation", "filloutUKRD.Command", "opening_elevation.png", "Cập nhật thông thông tin cao độ bản vẽ formwork" })) as PushButton;
                    }
                }
                catch (Exception) { }
            }

            ribbonTool.AddSeparator();
            PushButton pushButtonPrint = ribbonTool.AddItem(create_pushdata(new List<string>() { "Auto_Prin_PDF.dll", "Super Print", "Print.Command", "printer.png", "In PDF, DWG theo setup sẵn" })) as PushButton;

            PushButton pushButtonDraw = ribbonDraw.AddItem(create_pushdata(new List<string>() { "Draw_All.dll", "Super Draw", "Draw_All.Draw_All", "DrawAll.ico", "Dựng mô hình, phục vụ công tác layout formwork" })) as PushButton;
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