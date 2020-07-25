using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
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
using Visibility = System.Windows.Visibility;

namespace Draw_Opening
{
    public class SortStringNumbers : IComparer<string>
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern int StrCmpLogicalW(string x, string y);

        public int Compare(string x, string y)
        {
            return StrCmpLogicalW(x, y);
        }
    }

    //public class SortObjectNumbers : IComparer<Quatity>
    //{
    //    public int Compare(Quatity x, Quatity y)
    //    {
    //        return x.level.CompareTo(y.level);
    //    }
    //}

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

    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = (bool)value;
            boolValue = (parameter != null) ? !boolValue : boolValue;
            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ValueConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ListView thong_tin_detail = (ListView)value;
            CollectionView view_detail = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_detail.ItemsSource);
            return view_detail.Count;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SelectElementsFilter : ISelectionFilter
    {
        static List<string> CategoryName = new List<string>();

        public SelectElementsFilter(List<string> name)
        {
            CategoryName = name;
        }

        public bool AllowElement(Element e)
        {
            if (CategoryName.Contains(e.Category.Name))
                return true;
            return false;
        }

        public bool AllowReference(Reference r, XYZ p)
        {
            return false;
        }
    }

    public class RowConverterMulti : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            GridLength width = new GridLength(0);
            bool cad = (bool)values[0];
            bool revit = (bool)values[1];
            if (cad == true || revit == true)
            {
                width = new GridLength(550, GridUnitType.Star);
            }
            else
            {
                width = new GridLength(0);
            }
            return width;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class RowConverter : IValueConverter
    {
        public object Convert(object values, Type targetType, object parameter, CultureInfo culture)
        {
            GridLength width = new GridLength(0);
            bool cad = (bool)values;
            if (cad == true)
            {
                width = new GridLength(550, GridUnitType.Star);
            }
            else
            {
                width = new GridLength(0);
            }
            return width;
        }

        public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
