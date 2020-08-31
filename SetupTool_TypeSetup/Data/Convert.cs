using Autodesk.Revit.DB;
using SetupTool_TypeSetup.Data.Binding;
using SetupTool_TypeSetup.Data.BindingCompany;
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

namespace SetupTool_TypeSetup
{
    //public class IntToString : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return value.ToString();
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        int ret = 0;
    //        return int.TryParse((string)value, out ret) ? ret : 0;
    //    }
    //}

    //public class DoubleToString : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return value.ToString();
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return (Double)value;
    //    }
    //}

    class sort_data_family : IComparer
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern int StrCmpLogicalW(string x, string y);

        public int Compare(object x, object y)
        {
            data_family a = (data_family)x;
            data_family b = (data_family)y;
            return StrCmpLogicalW(a.ten_family_type, b.ten_family_type);
        }
    }

    class sort_data_materials : IComparer
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern int StrCmpLogicalW(string x, string y);

        public int Compare(object x, object y)
        {
            data_materials a = (data_materials)x;
            data_materials b = (data_materials)y;
            return StrCmpLogicalW(a.ten_cong_tac, b.ten_cong_tac);
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
            return StrCmpLogicalW(a.ten_parameter, b.ten_parameter);
        }
    }

    class sort_data_material : IComparer<data_material>
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern int StrCmpLogicalW(string x, string y);

        public int Compare(data_material x, data_material y)
        {
            return StrCmpLogicalW(x.name, y.name);
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

    class InvertedBoolenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }
    }

    class ConvertColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }
    }

    class NameConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            data_name_key descipline = (data_name_key)values[0];
            bool outhome = (bool)values[1];
            bool inhome = (bool)values[2];
            bool kc = (bool)values[3];
            data_name_key category = (data_name_key)values[4];
            string custom = (string)values[5];
            return (descipline != null ? descipline.key : "") + "_" + Source.list_position.First(x => x.name == (outhome ? "Ngoài Nhà" : (inhome ? "Trong Nhà" : "None"))).key + "_" + (category != null ? category.key : "") + "_" + custom;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[] { value };
        }
    }

    class CustomConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string text = "";
            //if (values[1] != null)
            //{
            //    if (CollectionViewSource.GetDefaultView(values[1]) != null)
            //    {
            //        if (CollectionViewSource.GetDefaultView(values[1]).Cast<data_parameters>().ToList().Count > 0)
            //        {

            //            text = string.Join("x",CollectionViewSource.GetDefaultView(values[1]).Cast<data_parameters>().OrderByDescending(x => x.ten_parameter).ToList().Where(x => x.group_parameter == "Dimensions").Select(x => x.gia_tri_parameter));
            //        }
            //    }
            //}
            if (values[0] != null)
            {
                if(values[0] is data_type)
                {
                    data_type type = (data_type)values[0];
                    var list_name = type.element_type.Name.Split('_').ToList();
                    list_name.RemoveRange(0, 3);
                    text += string.Join("_", list_name);
                }
            }
            return text;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new object[] { value };
        }
    }
}
