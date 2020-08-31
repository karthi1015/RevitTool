using ARC_SupportLayout.Code;
using ARC_SupportLayout.Code.External;
using ARC_SupportLayout.Code.Function;
using ARC_SupportLayout.Data;
using ARC_SupportLayout.Data.Binding;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ARC_SupportLayout.UI
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class UserControl1 : Window
    {
        UIApplication uiapp;
        UIDocument uidoc;
        Document doc;

        E_Update my_update;
        ExternalEvent e_update;

        string project_number;
        string block;
        string Class;
        string unit_length;
        string id_file;
        string user;

        ObservableCollection<data_width> my_width { get; set; }
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public UserControl1(UIApplication _uiapp)
        {
            InitializeComponent();
            uiapp = _uiapp;
            uidoc = uiapp.ActiveUIDocument;
            doc = uidoc.Document;

            my_update = new E_Update();
            e_update = ExternalEvent.Create(my_update);

            
            Function_Dau_Vao();
        }

        public void Function_Dau_Vao()
        {
            try
            {
                my_width = new ObservableCollection<data_width>();

                List<ElementType> data_type = new FilteredElementCollector(doc).WhereElementIsElementType().Cast<ElementType>().ToList();
                F_GetWidth.get_width(my_width, data_width, data_type);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                my_update.data_width = data_width;
                my_update.my_width = my_width;
                e_update.Raise();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void hight_lights_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if(data_width.SelectedItem != null)
                {
                    Selection selection = uidoc.Selection;
                    List<ElementId> ids = new List<ElementId>();
                    new FilteredElementCollector(doc, doc.ActiveView.Id).WhereElementIsNotElementType()
                        .Where(x => x.Category != null && x.Category.AllowsBoundParameters == true && x.Category.CategoryType.ToString() == "Model")
                        .Where(x => ((data_width)data_width.SelectedItem).types.Select(y => y.Name).ToList().Exists(y => y == x.Name)).ToList().ForEach(x => ids.Add(x.Id));
                    selection.SetElementIds(ids);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void select_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Selection selection = uidoc.Selection;
                var elements = selection.PickObjects(ObjectType.Element).Select(x => doc.GetElement(x)).ToList();
                List<data_width> data_type_support = new List<data_width>();
                foreach (Element element in elements)
                {
                    ElementType type = doc.GetElement(element.get_Parameter(BuiltInParameter.ELEM_TYPE_PARAM).AsElementId()) as ElementType;
                    data_type_support.Add(new data_width()
                    {
                        type = type,
                        width = type.Name
                    });
                }
               
                F_GetWidth.get_width(my_width, data_width, data_type_support.GroupBy(x => new { x.width}).Select(x => x.First().type).ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
