using ARC_Quatity.Data.Binding;
using Autodesk.Revit.DB;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ARC_Quatity.Code.Function
{
    class F_Chart
    {
        //----------------------------------------------------------
        public static void Show_Du_Lieu_Len_Chart(PieChart bieu_do_category, ObservableCollection<data_element_link> my_element_link, object DataContext, UserControl1 userControl1)
        {
            try
            {
                bieu_do_category.Series.Clear();

                List<string> category_name_check = new List<string>();
                foreach (data_element_link ele_link in my_element_link)
                {
                    Element ele = ele_link.cau_kien;
                    if (category_name_check.Contains(ele.Category.Name) == false)
                    {
                        bieu_do_category.Series.Add(new PieSeries
                        {
                            Title = ele.Category.Name,
                            Values = new ChartValues<ObservableValue> { new ObservableValue(my_element_link.Count(x => x.cau_kien.Category.Name == ele.Category.Name)) },
                            StrokeThickness = 1,
                            DataLabels = true,
                            FontSize = 12
                        });
                        category_name_check.Add(ele.Category.Name);
                    }
                }

                DataContext = userControl1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
