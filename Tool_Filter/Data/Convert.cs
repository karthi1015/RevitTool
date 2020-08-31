using Autodesk.Revit.DB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Tool_Filter.Data.Binding;
using Visibility = System.Windows.Visibility;

namespace Tool_Filter
{
    class sort_data_category : IComparer
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern int StrCmpLogicalW(string x, string y);

        public int Compare(object x, object y)
        {
            data_category a = (data_category)x;
            data_category b = (data_category)y;
            return StrCmpLogicalW(a.category_name, b.category_name);
        }
    }

    class sort_data_family : IComparer
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern int StrCmpLogicalW(string x, string y);

        public int Compare(object x, object y)
        {
            data_family a = (data_family)x;
            data_family b = (data_family)y;
            return StrCmpLogicalW(a.family_name, b.family_name);
        }
    }

    class sort_data_type : IComparer
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern int StrCmpLogicalW(string x, string y);

        public int Compare(object x, object y)
        {
            data_type a = (data_type)x;
            data_type b = (data_type)y;
            return StrCmpLogicalW(a.type_name, b.type_name);
        }
    }

    class sort_data_parameters : IComparer
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern int StrCmpLogicalW(string x, string y);

        public int Compare(object x, object y)
        {
            data_parameters a = (data_parameters)x;
            data_parameters b = (data_parameters)y;
            return StrCmpLogicalW(a.parameter_name, b.parameter_name);
        }
    }

    class sort_data_parameters_value : IComparer
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern int StrCmpLogicalW(string x, string y);

        public int Compare(object x, object y)
        {
            data_parameters_value a = (data_parameters_value)x;
            data_parameters_value b = (data_parameters_value)y;
            return StrCmpLogicalW(a.parameter_name, b.parameter_name);
        }
    }

    class SortStringNumbers : IComparer<string>
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern int StrCmpLogicalW(string x, string y);

        public int Compare(string x, string y)
        {
            return StrCmpLogicalW(x, y);
        }
    }

    class bool_visible_family : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visible = Visibility.Collapsed;
            if (value != null)
            {
                if (CollectionViewSource.GetDefaultView(value) != null)
                {
                    if (CollectionViewSource.GetDefaultView(value).Cast<data_family>().ToList().Count > 0)
                    {
                        visible = Visibility.Visible;
                    }
                    else
                    {
                        visible = Visibility.Collapsed;
                    }
                }
            }
            return visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new object();
        }
    }

    class bool_visible_type : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visible = Visibility.Collapsed;
            if (value != null)
            {
                if (CollectionViewSource.GetDefaultView(value) != null)
                {
                    if (CollectionViewSource.GetDefaultView(value).Cast<data_type>().ToList().Count > 0)
                    {
                        visible = Visibility.Visible;
                    }
                    else
                    {
                        visible = Visibility.Collapsed;
                    }
                }
            }
            return visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new object();
        }
    }

    class bool_visible_parameter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visible = Visibility.Collapsed;
            if (value != null)
            {
                if (CollectionViewSource.GetDefaultView(value) != null)
                {
                    if (CollectionViewSource.GetDefaultView(value).Cast<data_parameters>().ToList().Count > 0)
                    {
                        visible = Visibility.Visible;
                    }
                    else
                    {
                        visible = Visibility.Collapsed;
                    }
                }
            }
            return visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new object();
        }
    }

    class bool_visible_parameter_value : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visible = Visibility.Collapsed;
            if (value != null)
            {
                if (CollectionViewSource.GetDefaultView(value) != null)
                {
                    if (CollectionViewSource.GetDefaultView(value).Cast<data_parameters_value>().ToList().Count > 0)
                    {
                        visible = Visibility.Visible;
                    }
                    else
                    {
                        visible = Visibility.Collapsed;
                    }
                }
            }
            return visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new object();
        }
    }
}
