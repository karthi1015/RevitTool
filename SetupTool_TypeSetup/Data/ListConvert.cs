using System;
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

    public class SortStringNumbers : IComparer<string>
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern int StrCmpLogicalW(string x, string y);

        public int Compare(string x, string y)
        {
            return StrCmpLogicalW(x, y);
        }
    }

    public class InvertedBoolenConverter : IValueConverter
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

    public class ConvertColor : IValueConverter
    {
        ListSource myListSource;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }
    }

    public class TextConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string text = "";
            bool arc = (bool)values[0];
            bool str = (bool)values[1];
            var ptc = "PTC_";
            if (arc) text = ptc + "A";
            else if (str) text = ptc + "S";
            else text = ptc + "M";

            Data item = (Data)values[2];
            string text1 = "";

            if (item != null)
            {
                text1 = text + "_" + item.ten_type + "_";
            }
            else
            {
                text1 = text + "_";
            }

            return text1;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class GroupItemStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {

            Style s = null;

            CollectionViewGroup group = item as CollectionViewGroup;
            Window window = Application.Current.MainWindow;

            if (!group.IsBottomLevel)
            {
                s = window.FindResource("FirstLevel") as Style;
            }
            //else
            //{
            //    s = window.FindResource("SecondLevel") as Style;
            //}

            return s;
        }
    }
}
