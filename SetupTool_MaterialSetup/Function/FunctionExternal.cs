#region Namespaces
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ListView = System.Windows.Controls.ListView;
using TextBox = System.Windows.Controls.TextBox;
using ComboBox = System.Windows.Controls.ComboBox;
using CheckBox = System.Windows.Controls.CheckBox;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using System.Collections.ObjectModel;
using System.Linq;
#endregion

namespace SetupTool_MaterialSetup
{
    //ExternalEventClass myExampleDraw;
    //ExternalEvent Draw;

    //myExampleDraw = new ExternalEventClass();
    //Draw = ExternalEvent.Create(myExampleDraw);

    //Draw.Raise();
    public class ExternalEventClass : IExternalEventHandler
    {
        public string command { get; set; }
        public string path { get; set; }

        public ListView thong_tin_vat_lieu_project { get; set; }

        public TextBox ten_vat_lieu { get; set; }

        public TextBox ma_cong_tac { get; set; }

        public ComboBox don_vi { get; set; }

        public CheckBox use_appearence { get; set; }

        public TextBox tranparency_value { get; set; }

        public Color color_Shading { get; set; }
        public Color color_Surface { get; set; }
        public Color color_Cut { get; set; }

        public FillPatternElement fillPattern_Surface { get; set; }
        public FillPatternElement fillPattern_Cut { get; set; }

        public TextBox ton_value { get; set; }

        public ObservableCollection<Material_Project> myMaterial_Project { get; set; }
        public ObservableCollection<Material_Factor> myMaterial_Project_Factor { get; set; }
        public ObservableCollection<Material_Project> data_create { get; set; }

        public All_Data myAll_Data { get; set; }

        ListSource mySource;
        FunctionSupoort myFunctionSupport;

        public void Execute(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            mySource = new ListSource();
            myFunctionSupport = new FunctionSupoort();

            try
            {
                Transaction transaction = new Transaction(doc);
                transaction.Start("Material");

                if (command == "Modify")
                {
                    string result = Sua_Thong_Tin_Vat_Lieu_Project(uiapp, doc);
                    if (result == "S")
                    {
                        Get_Data_Factor(doc);
                        MessageBox.Show("Modify Success!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

                if (command == "Modify_Ton")
                {
                    string result = Sua_Ton_Kg_Vat_Lieu_Project(uiapp, doc);
                    if (result == "S")
                    {
                        foreach(Material_Project material_Project in myMaterial_Project)
                        {
                            material_Project.ton = ton_value.Text;
                        }
                        MessageBox.Show("Modify Success!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    ton_value.Text = myFunctionSupport.Check_Para_And_Get_Para(myMaterial_Project[0].material_project, myAll_Data.list_material_para_data[2].material_para_guid, myAll_Data.list_material_para_data[2].material_para_name);
                }

                if (command == "Create")
                {
                    string result = Tao_Vat_Lieu_Moi_Project(uiapp, doc);
                    if (result == "S")
                    {
                        Get_Data_Factor(doc);
                        MessageBox.Show("Create Success!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

                if (command == "Delete")
                {
                    string result = Xoa_Vat_Lieu_Project(uiapp, doc);
                    if (result == "S")
                    {
                        Get_Data_Factor(doc);
                        MessageBox.Show("Delete Success!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

                if (command == "Excel")
                {
                    string result = Tao_Vat_Lieu_Moi_Project_Excel(uiapp, doc);
                    if (result == "S")
                    {
                        Get_Data_Factor(doc);
                        MessageBox.Show("Create Success!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return;
        }
        public string GetName()
        {
            return "External Event Example";
        }

        //----------------------------------------------------------
        public void Get_Data_Factor(Document doc)
        {
            try
            {
                myMaterial_Project_Factor.Clear();
                List<Element> list_material = new FilteredElementCollector(doc).OfClass(typeof(Material)).ToList();
                ObservableCollection<Material_Project> support = new ObservableCollection<Material_Project>();

                foreach (Material material in list_material)
                {
                    support.Add(new Material_Project()
                    {
                        material_project = material,
                        ton = Convert.ToDouble(myFunctionSupport.Check_Para_And_Get_Para(material, myAll_Data.list_material_para_data[2].material_para_guid, myAll_Data.list_material_para_data[2].material_para_name)).ToString(),
                    });
                }

                new ObservableCollection<Material_Factor>(support
                    .GroupBy(x => new
                    {
                        x.ton
                    }).Select(y => new Material_Factor()
                    {
                        ton_value = y.Key.ton,
                        list_vat_lieu = y.Select(z => z.material_project).ToList(),
                        count = y.Select(z => z.material_project).ToList().Count()
                    }).OrderBy(z => z.ton_value)).ToList().ForEach(i => myMaterial_Project_Factor.Add(i));

            }
            catch (Exception)
            {

            }
        }

        //----------------------------------------------------------
        public string Sua_Thong_Tin_Vat_Lieu_Project(UIApplication uiapp,Document doc)
        {
            string result = "F";
            try
            {
                Material_Project item = (Material_Project)thong_tin_vat_lieu_project.SelectedItem;
                if (thong_tin_vat_lieu_project.SelectedItem != null)
                {
                    if (!string.IsNullOrEmpty(ten_vat_lieu.Text))
                    {
                        if (myMaterial_Project.Any(x => x.ten_vat_lieu_project == ten_vat_lieu.Text) == false || item.ten_vat_lieu_project == ten_vat_lieu.Text)
                        {
                            if (myMaterial_Project.Any(x => x.ma_cong_tac_project == ma_cong_tac.Text) == false || item.ma_cong_tac_project == ma_cong_tac.Text)
                            {
                                Material material = item.material_project;

                                Data donvi = (Data)don_vi.SelectedItem;
                                Color color_Shading_list = item.mau_vat_lieu;
                                int tranparency = item.do_trong_suot_vat_lieu;
                                Color color_Surface_list = item.mau_surface;
                                Color color_Cut_list = item.mau_cut;
                                ElementId fillPattern_Surface_list = item.id_surface;
                                ElementId fillPattern_Cut_list = item.id_cut;

                                if (use_appearence.IsChecked == false)
                                {
                                    if (color_Shading != null)
                                    {
                                        color_Shading_list = color_Shading;
                                    }
                                    tranparency = Convert.ToInt32(tranparency_value.Text);
                                }

                                if (color_Surface != null) color_Surface_list = color_Surface;
                                if (fillPattern_Surface != null) fillPattern_Surface_list = fillPattern_Surface.Id;

                                if (color_Cut != null) color_Cut_list = color_Cut;
                                if (fillPattern_Cut != null) fillPattern_Cut_list = fillPattern_Cut.Id;

                                string ton = item.ton;
                                if (!string.IsNullOrEmpty(ton_value.Text)) ton = ton_value.Text; 

                                List<Material_Project> data_material_modify = new List<Material_Project>();
                                data_material_modify.Add(new Material_Project()
                                {
                                    material_project = material,
                                    ma_cong_tac_project = ma_cong_tac.Text,
                                    ten_vat_lieu_project = ten_vat_lieu.Text,
                                    don_vi_project = donvi.single_value,
                                    user = uiapp.Application.Username,
                                    time = DateTime.Now.ToString(),
                                    mau_vat_lieu = color_Shading_list,
                                    do_trong_suot_vat_lieu = tranparency,
                                    id_surface = fillPattern_Surface_list,
                                    mau_surface = color_Surface_list,
                                    id_cut = fillPattern_Cut_list,
                                    mau_cut = color_Cut_list,
                                    ton = ton,
                                    color = myFunctionSupport.Check_Color(ma_cong_tac.Text, myAll_Data),
                                    color_sort = myFunctionSupport.Check_Color(ma_cong_tac.Text, myAll_Data).ToString()
                                });

                                material.Name = data_material_modify[0].ten_vat_lieu_project;
                                myFunctionSupport.SetDataStorage(material, myAll_Data.list_material_para_data[0].material_para_guid, myAll_Data.list_material_para_data[0].material_para_name, data_material_modify[0].ma_cong_tac_project);
                                myFunctionSupport.SetDataStorage(material, myAll_Data.list_material_para_data[1].material_para_guid, myAll_Data.list_material_para_data[1].material_para_name, data_material_modify[0].don_vi_project);
                                myFunctionSupport.SetDataStorage(material, myAll_Data.list_material_para_data[2].material_para_guid, myAll_Data.list_material_para_data[2].material_para_name, data_material_modify[0].ton);
                                myFunctionSupport.SetDataStorage(material, myAll_Data.list_material_para_data[3].material_para_guid, myAll_Data.list_material_para_data[3].material_para_name, data_material_modify[0].user);
                                myFunctionSupport.SetDataStorage(material, myAll_Data.list_material_para_data[4].material_para_guid, myAll_Data.list_material_para_data[4].material_para_name, data_material_modify[0].time);
                                material.Color = data_material_modify[0].mau_vat_lieu;
                                material.Transparency = data_material_modify[0].do_trong_suot_vat_lieu;
                                material.SurfacePatternId = data_material_modify[0].id_surface;
                                material.SurfacePatternColor = data_material_modify[0].mau_surface;
                                material.CutPatternId = data_material_modify[0].id_cut;
                                material.CutPatternColor = data_material_modify[0].mau_cut;
                                myFunctionSupport.SetDataStorage(material, myAll_Data.list_material_para_data[2].material_para_guid, myAll_Data.list_material_para_data[2].material_para_name, data_material_modify[0].ton);

                                myFunctionSupport.SetColorAppearanceAsset(doc, material);

                                myMaterial_Project.Remove(item);
                                myMaterial_Project.Add(data_material_modify[0]);
                                result = "S";
                            }
                            else
                            {
                                MessageBox.Show("\"Mã Công Tác\" không được trùng nhau!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("\"Tên Vật Liệu\" không được trùng nhau!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("\"Tên Vật Liệu\" không được để trống!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Hãy chọn vật liệu muốn chỉnh sửa!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }

        //----------------------------------------------------------
        public string Sua_Ton_Kg_Vat_Lieu_Project(UIApplication uiapp, Document doc)
        {
            string result = "F";
            try
            {
                foreach (Material_Factor material_factor in myMaterial_Project_Factor)
                {
                    string value = material_factor.ton_value;
                    foreach (Material material in material_factor.list_vat_lieu)
                    {
                        if (!string.IsNullOrEmpty(value))
                        {
                            myFunctionSupport.SetDataStorage(material, myAll_Data.list_material_para_data[2].material_para_guid, myAll_Data.list_material_para_data[2].material_para_name, value);
                            result = "S";
                        }
                    } 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }

        //----------------------------------------------------------
        public string Tao_Vat_Lieu_Moi_Project(UIApplication uiapp, Document doc)
        {
            string result = "F";
            try
            {
                if (!string.IsNullOrEmpty(ten_vat_lieu.Text))
                {
                    if (myMaterial_Project.Any(x => x.ten_vat_lieu_project == ten_vat_lieu.Text) == false)
                    {
                        if (myMaterial_Project.Any(x => x.ma_cong_tac_project == ma_cong_tac.Text) == false)
                        {
                            ElementId material_id_new = Material.Create(doc, ten_vat_lieu.Text);
                            Material material = doc.GetElement(material_id_new) as Material;

                            Data donvi = (Data)don_vi.SelectedItem;
                            Color color_Shading_list = new Color(0x78, 0x78, 0x78);
                            int tranparency = material.Transparency;
                            Color color_Surface_list = new Color(0x78, 0x78, 0x78);
                            Color color_Cut_list = new Color(0x78, 0x78, 0x78);
                            ElementId fillPattern_Surface_list = material.SurfacePatternId;
                            ElementId fillPattern_Cut_list = material.CutPatternId;

                            if (use_appearence.IsChecked == false)
                            {
                                if (color_Shading != null)
                                {
                                    color_Shading_list = color_Shading;
                                }
                                tranparency = Convert.ToInt32(tranparency_value.Text);
                            }

                            if (color_Surface != null) color_Surface_list = color_Surface;
                            if (fillPattern_Surface != null) fillPattern_Surface_list = fillPattern_Surface.Id;

                            if (color_Cut != null) color_Cut_list = color_Cut;
                            if (fillPattern_Cut != null) fillPattern_Cut_list = fillPattern_Cut.Id;

                            string ton = myAll_Data.list_unit_value_data[0].ToString();
                            if (!string.IsNullOrEmpty(ton_value.Text)) ton = ton_value.Text;

                            List<Material_Project> data_material_modify = new List<Material_Project>();
                            data_material_modify.Add(new Material_Project()
                            {
                                material_project = material,
                                ma_cong_tac_project = ma_cong_tac.Text,
                                ten_vat_lieu_project = ten_vat_lieu.Text,
                                don_vi_project = donvi.single_value,
                                user = uiapp.Application.Username,
                                time = DateTime.Now.ToString(),
                                mau_vat_lieu = color_Shading_list,
                                do_trong_suot_vat_lieu = tranparency,
                                id_surface = fillPattern_Surface_list,
                                mau_surface = color_Surface_list,
                                id_cut = fillPattern_Cut_list,
                                mau_cut = color_Cut_list,
                                ton = ton,
                                color = myFunctionSupport.Check_Color(ma_cong_tac.Text, myAll_Data),
                                color_sort =  myFunctionSupport.Check_Color(ma_cong_tac.Text, myAll_Data).ToString()
                            });

                            material.Name = data_material_modify[0].ten_vat_lieu_project;
                            myFunctionSupport.SetDataStorage(material, myAll_Data.list_material_para_data[0].material_para_guid, myAll_Data.list_material_para_data[0].material_para_name, data_material_modify[0].ma_cong_tac_project);
                            myFunctionSupport.SetDataStorage(material, myAll_Data.list_material_para_data[1].material_para_guid, myAll_Data.list_material_para_data[1].material_para_name, data_material_modify[0].don_vi_project);
                            myFunctionSupport.SetDataStorage(material, myAll_Data.list_material_para_data[3].material_para_guid, myAll_Data.list_material_para_data[3].material_para_name, data_material_modify[0].user);
                            myFunctionSupport.SetDataStorage(material, myAll_Data.list_material_para_data[4].material_para_guid, myAll_Data.list_material_para_data[4].material_para_name, data_material_modify[0].time);
                            material.Color = data_material_modify[0].mau_vat_lieu;
                            material.Transparency = data_material_modify[0].do_trong_suot_vat_lieu;
                            material.SurfacePatternId = data_material_modify[0].id_surface;
                            material.SurfacePatternColor = data_material_modify[0].mau_surface;
                            material.CutPatternId = data_material_modify[0].id_cut;
                            material.CutPatternColor = data_material_modify[0].mau_cut;
                            myFunctionSupport.SetDataStorage(material, myAll_Data.list_material_para_data[2].material_para_guid, myAll_Data.list_material_para_data[2].material_para_name, data_material_modify[0].ton);

                            myFunctionSupport.SetColorAppearanceAsset(doc, material);

                            myMaterial_Project.Add(data_material_modify[0]);
                            result = "S";
                        }
                        else
                        {
                            MessageBox.Show("\"Mã Công Tác\" không được trùng nhau!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("\"Tên Vật Liệu\" không được trùng nhau!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("\"Tên Vật Liệu\" không được để trống!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }

        //----------------------------------------------------------
        public string Xoa_Vat_Lieu_Project(UIApplication uiapp, Document doc)
        {
            string result = "F";
            try
            {
                List<ElementId> id_delete = new List<ElementId>();
                List<Material_Project> item_delete = new List<Material_Project>();
                if (thong_tin_vat_lieu_project.SelectedItems.Count > 0)
                {
                    for (var i = 0; i < thong_tin_vat_lieu_project.SelectedItems.Count; i++)
                    {
                        Material_Project item = (Material_Project)thong_tin_vat_lieu_project.SelectedItems[i];
                        id_delete.Add(item.material_project.Id);
                        item_delete.Add(item);
                    }
                }
                else
                {
                    MessageBox.Show("Hãy chọn vật liệu muốn xóa!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                if (id_delete.Count > 0)
                {
                    doc.Delete(id_delete);
                    foreach (Material_Project material_Project in item_delete)
                    {
                        myMaterial_Project.Remove(material_Project);
                    }
                    result = "S";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }

        //----------------------------------------------------------
        public string Tao_Vat_Lieu_Moi_Project_Excel(UIApplication uiapp, Document doc)
        {
            string result = "F";
            try
            {
                string name_error = "";
                foreach (Material_Project data in data_create)
                {
                    try
                    {
                        ElementId material_id_new = Material.Create(doc, data.ten_vat_lieu_project);
                        Material material = doc.GetElement(material_id_new) as Material;

                        Color color_Shading_list = new Color(0x78, 0x78, 0x78);
                        int tranparency = material.Transparency;
                        Color color_Surface_list = new Color(0x78, 0x78, 0x78);
                        Color color_Cut_list = new Color(0x78, 0x78, 0x78);
                        ElementId fillPattern_Surface_list = material.SurfacePatternId;
                        ElementId fillPattern_Cut_list = material.CutPatternId;

                        if (data.mau_vat_lieu != null)
                        {
                            color_Shading_list = data.mau_vat_lieu;
                        }
                        tranparency = data.do_trong_suot_vat_lieu;

                        if (data.mau_surface != null) color_Surface_list = data.mau_surface;
                        if (data.id_surface.IntegerValue != -1) fillPattern_Surface_list = data.id_surface;

                        if (data.mau_cut != null) color_Cut_list = data.mau_cut;
                        if (data.id_cut.IntegerValue != -1) fillPattern_Cut_list = data.id_cut;

                        string ton = myAll_Data.list_unit_value_data[0].ToString();
                        if (!string.IsNullOrEmpty(data.ton)) ton = data.ton;

                        List<Material_Project> data_material_modify = new List<Material_Project>();
                        data_material_modify.Add(new Material_Project()
                        {
                            material_project = material,
                            ma_cong_tac_project = data.ma_cong_tac_project,
                            ten_vat_lieu_project = data.ten_vat_lieu_project,
                            don_vi_project = data.don_vi_project,
                            user = data.user,
                            time = data.time,
                            mau_vat_lieu = color_Shading_list,
                            do_trong_suot_vat_lieu = tranparency,
                            id_surface = fillPattern_Surface_list,
                            mau_surface = color_Surface_list,
                            id_cut = fillPattern_Cut_list,
                            mau_cut = color_Cut_list,
                            ton = ton,
                            color = myFunctionSupport.Check_Color(data.ma_cong_tac_project, myAll_Data),
                            color_sort = myFunctionSupport.Check_Color(data.ma_cong_tac_project, myAll_Data).ToString()
                        });

                        material.Name = data_material_modify[0].ten_vat_lieu_project;
                        myFunctionSupport.SetDataStorage(material, myAll_Data.list_material_para_data[0].material_para_guid, myAll_Data.list_material_para_data[0].material_para_name, data_material_modify[0].ma_cong_tac_project);
                        myFunctionSupport.SetDataStorage(material, myAll_Data.list_material_para_data[1].material_para_guid, myAll_Data.list_material_para_data[1].material_para_name, data_material_modify[0].don_vi_project);
                        myFunctionSupport.SetDataStorage(material, myAll_Data.list_material_para_data[3].material_para_guid, myAll_Data.list_material_para_data[3].material_para_name, data_material_modify[0].user);
                        myFunctionSupport.SetDataStorage(material, myAll_Data.list_material_para_data[4].material_para_guid, myAll_Data.list_material_para_data[4].material_para_name, data_material_modify[0].time);
                        material.Color = data_material_modify[0].mau_vat_lieu;
                        material.Transparency = data_material_modify[0].do_trong_suot_vat_lieu;
                        material.SurfacePatternId = data_material_modify[0].id_surface;
                        material.SurfacePatternColor = data_material_modify[0].mau_surface;
                        material.CutPatternId = data_material_modify[0].id_cut;
                        material.CutPatternColor = data_material_modify[0].mau_cut;
                        myFunctionSupport.SetDataStorage(material, myAll_Data.list_material_para_data[2].material_para_guid, myAll_Data.list_material_para_data[2].material_para_name, data_material_modify[0].ton);

                        myFunctionSupport.SetColorAppearanceAsset(doc, material);

                        myMaterial_Project.Add(data_material_modify[0]);
                        result = "S";
                    }
                    catch (Exception)
                    {
                        name_error += data.ten_vat_lieu_project + "\n";
                    }
                }
                if (!string.IsNullOrEmpty(name_error)) MessageBox.Show("Những vật liệu không tạo được do có lỗi hoặc trùng với vật liệu có trong dự án :\n" + name_error, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }
    }
}
