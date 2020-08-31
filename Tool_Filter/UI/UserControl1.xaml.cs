using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Tool_Filter.Code.External;
using Tool_Filter.Code.Function;
using Tool_Filter.Data.Binding;

namespace Tool_Filter
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class UserControl1 : Window
    {
        UIApplication uiapp;
        UIDocument uidoc;
        Document doc;

        E_HighLights my_high_lights;
        ExternalEvent e_high_lights;
        E_Hide my_hide;
        ExternalEvent e_hide;
        E_Isolate my_isolate;
        ExternalEvent e_isolate;
        E_Refresh my_refresh;
        ExternalEvent e_refresh;
        E_DuplicateView my_duplicate_view;
        ExternalEvent e_duplicate_view;

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        ObservableCollection<data_category> my_category { get; set; }
        ObservableCollection<data_family> my_family { get; set; }
        ObservableCollection<data_type> my_type { get; set; }
        ObservableCollection<data_parameters> my_parameters { get; set; }
        ObservableCollection<data_parameters_value> my_parameters_value { get; set; }

        ObservableCollection<data_element> my_element_quickly { get; set; }
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public string path = "";
        public UserControl1(UIApplication _uiapp)
        {
            InitializeComponent();

            uiapp = _uiapp;
            uidoc = uiapp.ActiveUIDocument;
            doc = uidoc.Document;

            register_external();
            Function_Dau_Vao();
        }

        //----------------------------------------------------------
        public void Function_Dau_Vao()
        {
            try
            {
                my_category = new ObservableCollection<data_category>();
                my_family = new ObservableCollection<data_family>();
                my_type = new ObservableCollection<data_type>();
                my_parameters = new ObservableCollection<data_parameters>();
                my_parameters_value = new ObservableCollection<data_parameters_value>();

                F_GetCategory.distint_category(doc, my_category, thong_tin_category, new FilteredElementCollector(doc, doc.ActiveView.Id).WhereElementIsNotElementType().ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //----------------------------------------------------------
        private bool Filter(object item)
        {
            string text = "";
            if (string.IsNullOrEmpty(search.Text))
                return true;
            else
            {
                if (cate.IsChecked == true) text = (item as data_category).category_name;
                else if (family.IsChecked == true) text = (item as data_family).family_name;
                else if (type.IsChecked == true) text = (item as data_type).type_name;
                else if (para.IsChecked == true) text = (item as data_parameters).parameter_name;
                else text = (item as data_parameters_value).parameter_value;
            }
            return text.IndexOf(search.Text, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        //----------------------------------------------------------
        void register_external()
        {
            my_high_lights = new E_HighLights();
            e_high_lights = ExternalEvent.Create(my_high_lights);

            my_hide = new E_Hide();
            e_hide = ExternalEvent.Create(my_hide);

            my_isolate = new E_Isolate();
            e_isolate = ExternalEvent.Create(my_isolate);

            my_refresh = new E_Refresh();
            e_refresh = ExternalEvent.Create(my_refresh);

            my_duplicate_view = new E_DuplicateView();
            e_duplicate_view = ExternalEvent.Create(my_duplicate_view);
        }

        #region function checkbox
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Filter_By_Category(object sender, RoutedEventArgs e)
        {
            my_family = new ObservableCollection<data_family>();
            F_GetFamily.distint_family(my_category, thong_tin_category, my_family, thong_tin_family);

            my_type = new ObservableCollection<data_type>();
            F_GetElementType.distint_type(my_family, thong_tin_family, my_type, thong_tin_type);

            my_parameters = new ObservableCollection<data_parameters>();
            F_GetParameters.distint_parameters(doc, my_type, thong_tin_type, my_parameters, thong_tin_parameter);

            my_parameters_value = new ObservableCollection<data_parameters_value>();
            F_GetParametersValue.distint_parameter_values(doc, my_parameters, thong_tin_parameter, my_parameters_value, gia_tri_parameter);

            All_Check();
        }

        //---------------------------------------------------------
        private void Filter_By_Family(object sender, RoutedEventArgs e)
        {
            my_type = new ObservableCollection<data_type>();
            F_GetElementType.distint_type(my_family, thong_tin_family, my_type, thong_tin_type);

            my_parameters = new ObservableCollection<data_parameters>();
            F_GetParameters.distint_parameters(doc, my_type, thong_tin_type, my_parameters, thong_tin_parameter);

            my_parameters_value = new ObservableCollection<data_parameters_value>();
            F_GetParametersValue.distint_parameter_values(doc, my_parameters, thong_tin_parameter, my_parameters_value, gia_tri_parameter);

            All_Check();
        }



        //---------------------------------------------------------
        private void Filter_Type(object sender, RoutedEventArgs e)
        {
            my_parameters = new ObservableCollection<data_parameters>();
            F_GetParameters.distint_parameters(doc, my_type, thong_tin_type, my_parameters, thong_tin_parameter);

            my_parameters_value = new ObservableCollection<data_parameters_value>();
            F_GetParametersValue.distint_parameter_values(doc, my_parameters, thong_tin_parameter, my_parameters_value, gia_tri_parameter);

            All_Check();
        }



        //---------------------------------------------------------
        private void Filter_Parameter(object sender, RoutedEventArgs e)
        {
            my_parameters_value = new ObservableCollection<data_parameters_value>();
            F_GetParametersValue.distint_parameter_values(doc, my_parameters, thong_tin_parameter, my_parameters_value, gia_tri_parameter);

            All_Check();
        }

        //---------------------------------------------------------
        private void Filter_Value(object sender, RoutedEventArgs e)
        {
            All_Check();
        }
        #endregion

        #region control
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        List<ElementId> ids = new List<ElementId>();
        private void Hight_Lights_Filter(object sender, RoutedEventArgs e)
        {
            try
            {
                ids = new List<ElementId>();
                if (my_parameters_value.Where(x => x.check == true).ToList().Count() > 0)
                {
                    my_parameters_value.Where(x => x.check == true).Select(y => y.elements).ToList().ForEach(z => z.ForEach(i => ids.Add(i.Id)));
                }
                else if (my_parameters.Where(x => x.check == true).ToList().Count() > 0)
                {
                    my_parameters.Where(x => x.check == true).Select(y => y.elements).ToList().ForEach(z => z.ForEach(i => ids.Add(i.Id)));
                }
                else if (my_type.Where(x => x.check == true).ToList().Count() > 0)
                {
                    my_type.Where(x => x.check == true).Select(y => y.elements).ToList().ForEach(z => z.ForEach(i => ids.Add(i.Id)));
                }
                else if (my_family.Where(x => x.check == true).ToList().Count() > 0)
                {
                    my_family.Where(x => x.check == true).Select(y => y.elements).ToList().ForEach(z => z.ForEach(i => ids.Add(i.Id)));
                }
                else if (my_category.Where(x => x.check == true).ToList().Count() > 0)
                {
                    my_category.Where(x => x.check == true).Select(y => y.elements).ToList().ForEach(z => z.ForEach(i => ids.Add(i.Id)));
                }

                my_high_lights.ids = ids;
                e_high_lights.Raise();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        bool state = false;
        private void Hide(object sender, RoutedEventArgs e)
        {
            try
            {
                my_hide.ids = ids;
                e_hide.Raise();
                state = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        private void Hide_Isolate(object sender, RoutedEventArgs e)
        {
            try
            {
                my_isolate.ids = ids;
                e_isolate.Raise();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        private void Duplicate_View(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(view_name.Text))
                {
                    my_duplicate_view.ids = ids;
                    my_duplicate_view.state = state;
                    my_duplicate_view.view_name = view_name;
                    e_duplicate_view.Raise();
                }
                else
                {
                    MessageBox.Show("The view name not null or empty.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region check all
        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Check_All_Value(object sender, RoutedEventArgs e)
        {
            try
            {
                if (check_all_value.IsChecked == true) my_parameters_value.ToList().ForEach(y => y.check = true); else my_parameters_value.ToList().ForEach(y => y.check = false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            All_Check();
        }

        //----------------------------------------------------------
        private void Check_All_parameter(object sender, RoutedEventArgs e)
        {
            try
            {
                if (check_all_parameter.IsChecked == true) my_parameters.ToList().ForEach(y => y.check = true); else my_parameters.ToList().ForEach(y => y.check = false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            my_parameters_value = new ObservableCollection<data_parameters_value>();
            F_GetParametersValue.distint_parameter_values(doc, my_parameters, thong_tin_parameter, my_parameters_value, gia_tri_parameter);
            All_Check();
        }

        //----------------------------------------------------------
        private void Check_All_type(object sender, RoutedEventArgs e)
        {
            try
            {
                if (check_all_type.IsChecked == true) my_type.ToList().ForEach(y => y.check = true); else my_type.ToList().ForEach(y => y.check = false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            my_parameters = new ObservableCollection<data_parameters>();
            F_GetParameters.distint_parameters(doc, my_type, thong_tin_type, my_parameters, thong_tin_parameter);

            my_parameters_value = new ObservableCollection<data_parameters_value>();
            F_GetParametersValue.distint_parameter_values(doc, my_parameters, thong_tin_parameter, my_parameters_value, gia_tri_parameter);
            All_Check();
        }

        //----------------------------------------------------------
        private void Check_All_family(object sender, RoutedEventArgs e)
        {
            try
            {
                if (check_all_family.IsChecked == true) my_family.ToList().ForEach(y => y.check = true); else my_family.ToList().ForEach(y => y.check = false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            my_type = new ObservableCollection<data_type>();
            F_GetElementType.distint_type(my_family, thong_tin_family, my_type, thong_tin_type);

            my_parameters = new ObservableCollection<data_parameters>();
            F_GetParameters.distint_parameters(doc, my_type, thong_tin_type, my_parameters, thong_tin_parameter);

            my_parameters_value = new ObservableCollection<data_parameters_value>();
            F_GetParametersValue.distint_parameter_values(doc, my_parameters, thong_tin_parameter, my_parameters_value, gia_tri_parameter);
            All_Check();
        }

        //----------------------------------------------------------
        private void Check_All_category(object sender, RoutedEventArgs e)
        {
            try
            {
                if (check_all_category.IsChecked == true) my_category.ToList().ForEach(y => y.check = true); else my_category.ToList().ForEach(y => y.check = false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            my_family = new ObservableCollection<data_family>();
            F_GetFamily.distint_family(my_category, thong_tin_category, my_family, thong_tin_family);

            my_type = new ObservableCollection<data_type>();
            F_GetElementType.distint_type(my_family, thong_tin_family, my_type, thong_tin_type);

            my_parameters = new ObservableCollection<data_parameters>();
            F_GetParameters.distint_parameters(doc, my_type, thong_tin_type, my_parameters, thong_tin_parameter);

            my_parameters_value = new ObservableCollection<data_parameters_value>();
            F_GetParametersValue.distint_parameter_values(doc, my_parameters, thong_tin_parameter, my_parameters_value, gia_tri_parameter);
            All_Check();
        }

        //----------------------------------------------------------
        public void All_Check()
        {
            try
            {
                ListCollectionView view_cate = CollectionViewSource.GetDefaultView(thong_tin_category.ItemsSource) as ListCollectionView;
                ListCollectionView view_family = CollectionViewSource.GetDefaultView(thong_tin_family.ItemsSource) as ListCollectionView;
                ListCollectionView view_type = CollectionViewSource.GetDefaultView(thong_tin_type.ItemsSource) as ListCollectionView;
                ListCollectionView view_para = CollectionViewSource.GetDefaultView(thong_tin_parameter.ItemsSource) as ListCollectionView;
                ListCollectionView view_value = CollectionViewSource.GetDefaultView(gia_tri_parameter.ItemsSource) as ListCollectionView;
                view_cate.Filter = Filter;
                view_family.Filter = Filter;
                view_type.Filter = Filter;
                view_para.Filter = Filter;
                view_value.Filter = Filter;

                if (my_category.Count() > 0 && my_category.Any(x => x.check != true) == false) check_all_category.IsChecked = true; else check_all_category.IsChecked = false;
                if (my_family.Count() > 0 && my_family.Any(x => x.check != true) == false) check_all_family.IsChecked = true; else check_all_family.IsChecked = false;
                if (my_type.Count() > 0 && my_type.Any(x => x.check != true) == false) check_all_type.IsChecked = true; else check_all_type.IsChecked = false;
                if (my_parameters.Count() > 0 && my_parameters.Any(x => x.check != true) == false) check_all_parameter.IsChecked = true; else check_all_parameter.IsChecked = false;
                if (my_parameters_value.Count() > 0 && my_parameters_value.Any(x => x.check != true) == false) check_all_value.IsChecked = true; else check_all_value.IsChecked = false;

                thong_tin_category.Items.Refresh();
                thong_tin_family.Items.Refresh();
                thong_tin_type.Items.Refresh();
                thong_tin_parameter.Items.Refresh();
                gia_tri_parameter.Items.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Custom_Select(object sender, RoutedEventArgs e)
        {
            try
            {
                Selection selection = uidoc.Selection;
                var element_list = selection.PickObjects(ObjectType.Element).Select(x => doc.GetElement(x)).ToList();

                my_category = new ObservableCollection<data_category>();
                my_family = new ObservableCollection<data_family>();
                my_type = new ObservableCollection<data_type>();
                my_parameters = new ObservableCollection<data_parameters>();
                my_parameters_value = new ObservableCollection<data_parameters_value>();

                F_GetCategory.distint_category(doc, my_category, thong_tin_category, element_list);
                F_GetFamily.distint_family(my_category, thong_tin_category, my_family, thong_tin_family);
                F_GetElementType.distint_type(my_family, thong_tin_family, my_type, thong_tin_type);
                F_GetParameters.distint_parameters(doc, my_type, thong_tin_type, my_parameters, thong_tin_parameter);
                F_GetParametersValue.distint_parameter_values(doc, my_parameters, thong_tin_parameter, my_parameters_value, gia_tri_parameter);
                All_Check();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Refesh(object sender, RoutedEventArgs e)
        {
            try
            {
                my_category = new ObservableCollection<data_category>();
                my_family = new ObservableCollection<data_family>();
                my_type = new ObservableCollection<data_type>();
                my_parameters = new ObservableCollection<data_parameters>();
                my_parameters_value = new ObservableCollection<data_parameters_value>();

                F_GetCategory.distint_category(doc, my_category, thong_tin_category, new FilteredElementCollector(doc, doc.ActiveView.Id).WhereElementIsNotElementType().ToList());
                F_GetFamily.distint_family(my_category, thong_tin_category, my_family, thong_tin_family);
                F_GetElementType.distint_type(my_family, thong_tin_family, my_type, thong_tin_type);
                F_GetParameters.distint_parameters(doc, my_type, thong_tin_type, my_parameters, thong_tin_parameter);
                F_GetParametersValue.distint_parameter_values(doc, my_parameters, thong_tin_parameter, my_parameters_value, gia_tri_parameter);
                All_Check();

                e_refresh.Raise();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void change_search_by(object sender, RoutedEventArgs e)
        {
            if (cate.IsChecked == true) CollectionViewSource.GetDefaultView(thong_tin_category.ItemsSource).Refresh();
            else if (family.IsChecked == true) CollectionViewSource.GetDefaultView(thong_tin_family.ItemsSource).Refresh();
            else if (type.IsChecked == true) CollectionViewSource.GetDefaultView(thong_tin_type.ItemsSource).Refresh();
            else if (para.IsChecked == true) CollectionViewSource.GetDefaultView(thong_tin_parameter.ItemsSource).Refresh();
            else CollectionViewSource.GetDefaultView(gia_tri_parameter.ItemsSource).Refresh();
        }

        private void search_by_text_change(object sender, TextChangedEventArgs e)
        {
            if (cate.IsChecked == true) CollectionViewSource.GetDefaultView(thong_tin_category.ItemsSource).Refresh();
            else if (family.IsChecked == true) CollectionViewSource.GetDefaultView(thong_tin_family.ItemsSource).Refresh();
            else if (type.IsChecked == true) CollectionViewSource.GetDefaultView(thong_tin_type.ItemsSource).Refresh();
            else if (para.IsChecked == true) CollectionViewSource.GetDefaultView(thong_tin_parameter.ItemsSource).Refresh();
            else CollectionViewSource.GetDefaultView(gia_tri_parameter.ItemsSource).Refresh();
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void quickly_filter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(parameter_name.Text) && !string.IsNullOrEmpty(parameter_value.Text))
                {
                    Parameter_Quickly_Filter();
                    var data = my_element_quickly.Where(x => x.parameter_name == parameter_name.Text && x.parameter_value == parameter_value.Text).ToList();
                    if (data.Count() > 0) ids = data.First().ids;

                    my_high_lights.ids = ids;
                    e_high_lights.Raise();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void Parameter_Quickly_Filter()
        {
            try
            {
                my_element_quickly = new ObservableCollection<data_element>();
                List<data_element> my_element = new List<data_element>();
                foreach (Element element in new FilteredElementCollector(doc, doc.ActiveView.Id).WhereElementIsNotElementType().ToList())
                {
                    foreach (Parameter para in element.Parameters)
                    {
                        my_element.Add(new data_element()
                        {
                            parameter_name = para.Definition.Name,
                            element = element,
                            parameter_value = para != null ? Support.Get_Parameter_Information(para, doc) : ""
                        });
                    }
                }
                my_element.GroupBy(x => new 
                { 
                    x.parameter_name, x.parameter_value
                }).Select(x => new data_element() 
                { 
                    parameter_name = x.Key.parameter_name,
                    parameter_value = x.Key.parameter_value,

                    ids = x.Select(y => y.element.Id).ToList()
                }).ToList().ForEach(x => my_element_quickly.Add(x));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}






