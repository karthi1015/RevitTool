using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MessageBox = System.Windows.MessageBox;

namespace Allplan_ParameterSupport
{
    class Support
    {
        public static void Add_Data_Listview(Element model, Element model_add, ObservableCollection<Data> list_data)
        {
            try
            {
                if (model.LookupParameter(Source.share_para_name) != null && model.LookupParameter(Source.share_para_level) != null)
                {
                    string name = "";
                    string level = "";
                    string text2 = model.UniqueId;
                    string Class = "";
                    Brush color = Source.color_error;
                    if (model.LookupParameter(Source.share_para_name).AsString() != null)
                    {
                        name = model.LookupParameter(Source.share_para_name).AsString();
                    }
                    if (model.LookupParameter(Source.share_para_level).AsString() != null)
                    {
                        level = model.LookupParameter(Source.share_para_level).AsString();
                    }
                    if (model.LookupParameter(Source.share_para_text2).AsString() != null)
                    {
                        text2 = model.LookupParameter(Source.share_para_text2).AsString();
                    }
                    if (model.LookupParameter(Source.share_para_class).AsString() != null)
                    {
                        Class = model.LookupParameter(Source.share_para_class).AsString();
                    }
                    if (model.LookupParameter(Source.share_para_name).AsString() != null && model.LookupParameter(Source.share_para_level).AsString() != null) color = Source.color;
                    list_data.Add(new Data()
                    {
                        ten_cau_kien = name,
                        level_cau_kien = level,
                        id_cau_kien = text2,
                        cau_kien = model_add,
                        color = color,
                        color_sort = color.ToString(),
                        descipline = Class
                    });
                }
            }
            catch (Exception)
            { 

            }
        }
    }
}
