using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Tool_ViewInformation.Code.External;
using Tool_ViewInformation.Code.Function;
using Tool_ViewInformation.Data.Binding;

namespace Tool_ViewInformation
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : Window
    {
        UIApplication uiapp;
        UIDocument uidoc;
        Document doc;

        string project_number;
        string block;
        string Class;

        E_HighLights my_high_lights;
        ExternalEvent e_high_lights;

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        ObservableCollection<data_quantity> my_quantity { get; set; }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public string path = "";
        public UserControl1(UIApplication _uiapp)
        {
            InitializeComponent();

            uiapp = _uiapp;
            uidoc = uiapp.ActiveUIDocument;
            doc = uidoc.Document;

            my_high_lights = new E_HighLights();
            e_high_lights = ExternalEvent.Create(my_high_lights);

            Function_Dau_Vao();
        }

        //------------------------------------------------------------------------------
        public void Function_Dau_Vao()
        {
            try
            {
                List<string> file_name = doc.Title.Split('_').ToList();
                project_number = doc.ProjectInformation.Number;
                block = doc.ProjectInformation.BuildingName;
                Class = doc.ProjectInformation.LookupParameter("Class") == null ? "" : doc.ProjectInformation.LookupParameter("Class").AsString();
                if (string.IsNullOrEmpty(Class)) MessageBox.Show("Share Parameter Class not found", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);

                if (file_name.Count() > 3)
                {
                    List<string> format = new List<string>();
                    if (project_number != file_name[0]) format.Add("Project Number");

                    if (block != file_name[1]) format.Add("Block");

                    if (Class != file_name[3]) format.Add("Class");

                    if (format.Count() == 0)
                    {

                    }
                    else
                    {
                        MessageBox.Show(string.Format("Data is incorrect.\nPlease check {0} and try again!", string.Join(",", format)), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                        this.Close();
                    }
                }
                else
                {
                    MessageBox.Show("File name is incorrect. Please check and try again!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //------------------------------------------------------------------------------
        private void Select_Element(object sender, RoutedEventArgs e)
        {
            try
            {
                Selection selection = uidoc.Selection;
                var reference = selection.PickObjects(ObjectType.Element);

                if (reference != null)
                {
                    Show_Khoi_Luong_Len_ListView(reference.Select(x => doc.GetElement(x)).ToList());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //------------------------------------------------------------------------------
        private void Search_Element(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(search_material_company.Text))
                { 
                    Element element = null;
                    element = unique_id.IsChecked == false ?
                    new FilteredElementCollector(doc).WhereElementIsNotElementType().Where(x => x.Id.ToString() == search_material_company.Text).First() :
                    new FilteredElementCollector(doc).WhereElementIsNotElementType()
                    .Where(y => (y.LookupParameter(Source.share_para_text2) != null && y.LookupParameter(Source.share_para_text2).AsString() == search_material_company.Text) || 
                    y.UniqueId.ToString() == search_material_company.Text).First();


                    if (element != null)
                    {
                        my_high_lights.ids = new List<ElementId>() { element.Id };
                        e_high_lights.Raise();
                        Show_Khoi_Luong_Len_ListView(new List<Element>() { element});
                    }
                    else
                    {
                        MessageBox.Show("Cấu kiện không tồn tại hoặc thông tin tìm kiếm không đúng!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //------------------------------------------------------------------------------
        private void select_id_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (thong_tin_quantity.SelectedItem != null)
                {
                    data_quantity item = (data_quantity)thong_tin_quantity.SelectedItem;
                    search_material_company.Text = item.id_cau_kien;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        #region Show_Khoi_Luong_Len_ListView
        //----------------------------------------------------------
        public void Show_Khoi_Luong_Len_ListView(List<Element> elements)
        {
            try
            {
                my_quantity = new ObservableCollection<data_quantity>();

                List<Level> levels = new FilteredElementCollector(doc).OfClass(typeof(Level)).Cast<Level>().ToList();

                foreach (Element element in elements)
                {
                    ObservableCollection<data_quantity> my_quatity_item_support = new ObservableCollection<data_quantity>();
                    if (element is FamilyInstance)
                    {
                        FamilyInstance familyInstance = element as FamilyInstance;
                        F_GetQuantity.Support_Get_Khoi_Luong(doc, element, levels, Class, block, my_quatity_item_support);
                        List<ElementId> elements_child = familyInstance.GetSubComponentIds().ToList();
                        foreach (ElementId id in elements_child)
                        {
                            FamilyInstance model_child = doc.GetElement(id) as FamilyInstance;
                            F_GetQuantity.Support_Get_Khoi_Luong(doc, model_child, levels, Class, block, my_quatity_item_support);
                            Support_Show_Khoi_Luong_Len_ListView(model_child, levels, my_quatity_item_support, doc, Class, block);
                        }
                    }
                    else
                    {
                        F_GetQuantity.Support_Get_Khoi_Luong(doc, element, levels, Class, block, my_quatity_item_support);
                    }

                    my_quatity_item_support.ToList().ForEach(x => my_quantity.Add(x));
                }

                thong_tin_quantity.ItemsSource = my_quantity;

                CollectionView view_total = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_quantity.ItemsSource);
                PropertyGroupDescription groupDescription1 = new PropertyGroupDescription("id_cau_kien");
                view_total.GroupDescriptions.Add(groupDescription1);
                view_total.SortDescriptions.Add(new SortDescription("ma_cong_tac", ListSortDirection.Ascending));
                view_total.SortDescriptions.Add(new SortDescription("ten_vat_lieu", ListSortDirection.Ascending));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        void Support_Show_Khoi_Luong_Len_ListView(FamilyInstance model_child, List<Level> levels, ObservableCollection<data_quantity> my_quatity_item_support, Document doc, string Class, string block)
        {
            try
            {
                if (model_child.GetSubComponentIds().ToList().Count() > 0)
                {
                    foreach (ElementId id_for in model_child.GetSubComponentIds())
                    {
                        model_child = doc.GetElement(id_for) as FamilyInstance;
                        F_GetQuantity.Support_Get_Khoi_Luong(doc, model_child, levels, Class, block, my_quatity_item_support);
                        Support_Show_Khoi_Luong_Len_ListView(model_child, levels, my_quatity_item_support, doc, Class, block);
                    }
                }
            }
            catch (Exception)
            {

            }
        }
        #endregion

        
    }
}
