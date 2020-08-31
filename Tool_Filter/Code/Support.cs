using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.ExtensibleStorage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MessageBox = System.Windows.MessageBox;

namespace Tool_Filter
{
    class Support
    {
        //----------------------------------------------------------
        public static string Check_Para_And_Get_Para(Material material, string guid, string name_para)
        {
            string value = null;
            try
            {
                Schema getSchema = Schema.Lookup(new Guid(guid));
                if (getSchema != null)
                {
                    Entity ent = material.GetEntity(getSchema);
                    if (ent.Schema != null)
                    {
                        value = ent.Get<string>(getSchema.GetField(name_para));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return value;
        }

        //----------------------------------------------------------
        public static string RemoveUnicode(string text)
        {
            const string FindText = "áàảãạâấầẩẫậăắằẳẵặđéèẻẽẹêếềểễệíìỉĩịóòỏõọôốồổỗộơớờởỡợúùủũụưứừửữựýỳỷỹỵÁÀẢÃẠÂẤẦẨẪẬĂẮẰẲẴẶĐÉÈẺẼẸÊẾỀỂỄỆÍÌỈĨỊÓÒỎÕỌÔỐỒỔỖỘƠỚỜỞỠỢÚÙỦŨỤƯỨỪỬỮỰÝỲỶỸỴ";
            const string ReplText = "aaaaaaaaaaaaaaaaadeeeeeeeeeeeiiiiiooooooooooooooooouuuuuuuuuuuyyyyyAAAAAAAAAAAAAAAAADEEEEEEEEEEEIIIIIOOOOOOOOOOOOOOOOOUUUUUUUUUUUYYYYY";
            int index = -1;
            char[] arrChar = FindText.ToCharArray();
            while ((index = text.IndexOfAny(arrChar)) != -1)
            {
                int index2 = FindText.IndexOf(text[index]);
                text = text.Replace(text[index], ReplText[index2]);
            }
            return text;
        }

        //----------------------------------------------------------
        public static string Get_Parameter_Information(Parameter para, Document doc)
        {
            string defValue = string.Empty;
            try
            {
                switch (para.StorageType)
                {
                    case StorageType.Double:
                        defValue = para.AsValueString();
                        break;
                    case StorageType.ElementId:
                        ElementId id = para.AsElementId();
                        defValue = id.IntegerValue >= 0 ? doc.GetElement(id).Name : id.IntegerValue.ToString();
                        break;
                    case StorageType.Integer:
                        defValue = (ParameterType.YesNo == para.Definition.ParameterType ? (para.AsInteger() == 0 ? "False" : "True") : para.AsInteger().ToString());
                        break;
                    case StorageType.String:
                        defValue = para.AsString() != null ? para.AsString() : "";
                        break;
                    default:
                        defValue = "Unexposed parameter.";
                        break;
                }
            }
            catch (Exception)
            {

            }
            return defValue;
        }
    }
}
