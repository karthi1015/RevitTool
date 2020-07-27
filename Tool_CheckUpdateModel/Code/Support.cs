using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using Tool_CheckUpdateModel.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.DB.IFC;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;
using Floor = Autodesk.Revit.DB.Floor;
using Parameter = Autodesk.Revit.DB.Parameter;

namespace Tool_CheckUpdateModel.Function
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
        public static void SetDataStorage(Material material, string guid, string name_para, string value)
        {
            try
            {
                SchemaBuilder sb1 = new SchemaBuilder(new Guid(guid));
                sb1.SetReadAccessLevel(AccessLevel.Public);
                sb1.SetWriteAccessLevel(AccessLevel.Public);
                sb1.SetVendorId("pentacons");
                sb1.SetSchemaName(name_para);

                FieldBuilder fieldUser = sb1.AddSimpleField(name_para, typeof(string));
                fieldUser.SetDocumentation("Set");
                Schema schema = sb1.Finish();
                Entity entity = new Entity(schema);

                Field field = schema.GetField(name_para);
                entity.Set(field, value);
                material.SetEntity(entity);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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
        public static void GetParameterValue(Parameter para, Parameter para_change)
        {
            try
            {
                if (para != null)
                {
                    if (para.Definition.Name == para_change.Definition.Name && para.IsReadOnly == false)
                    {
                        // Use different method to get parameter data according to the storage type
                        switch (para.StorageType)
                        {
                            case StorageType.Double:
                                //covert the number into Metric
                                para.Set(para_change.AsDouble());
                                break;
                            case StorageType.ElementId:
                                //find out the name of the element
                                para.Set(para_change.AsElementId());
                                break;
                            case StorageType.Integer:
                                para.Set(para_change.AsInteger());
                                break;
                            case StorageType.String:
                                para.Set(para_change.AsString());
                                break;
                            default:
                                //defValue = "Unexposed parameter.";
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public static string GetParameterInformation(Parameter para, Document document)
        {
            string defValue = string.Empty;
            try
            {
                if (para != null)
                {
                    // Use different method to get parameter data according to the storage type
                    switch (para.StorageType)
                    {
                        case StorageType.Double:
                            //covert the number into Metric
                            defValue = para.AsValueString();
                            break;
                        case StorageType.ElementId:
                            //find out the name of the element
                            Autodesk.Revit.DB.ElementId id = para.AsElementId();
                            if (id.IntegerValue >= 0)
                            {
                                defValue = document.GetElement(id).Name;
                            }
                            else
                            {
                                defValue = id.IntegerValue.ToString();
                            }
                            break;
                        case StorageType.Integer:
                            if (ParameterType.YesNo == para.Definition.ParameterType)
                            {
                                if (para.AsInteger() == 0)
                                {
                                    defValue = "False";
                                }
                                else
                                {
                                    defValue = "True";
                                }
                            }
                            else
                            {
                                defValue = para.AsInteger().ToString();
                            }
                            break;
                        case StorageType.String:
                            defValue = para.AsString();
                            break;
                        default:
                            defValue = "Unexposed parameter.";
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
            return defValue;
        }

        // Get Profile--------------------------------------------------------------------
        public static List<XYZ> Get_Profile(Document doc, Element element)
        {
            List<XYZ> list_Profile_point = new List<XYZ>();
            try
            {
                IList<Reference> sideFaces = null;
                if (element.Category.Name == "Walls") sideFaces = HostObjectUtils.GetSideFaces(element as Wall, ShellLayerType.Exterior);
                else sideFaces = HostObjectUtils.GetTopFaces(element as Floor);

                Element e2 = doc.GetElement(sideFaces[0]);
                Face face = e2.GetGeometryObjectFromReference(sideFaces[0]) as Face;

                // The normal of the wall external face.
                XYZ normal = face.ComputeNormal(new UV(0, 0));

                // Get edge loops as curve loops.
                IList<CurveLoop> curveLoops = face.GetEdgesAsCurveLoops();

                // ExporterIFCUtils class can also be used for 
                // non-IFC purposes. The SortCurveLoops method 
                // sorts curve loops (edge loops) so that the 
                // outer loops come first.
                IList<IList<CurveLoop>> curveLoopLoop = ExporterIFCUtils.SortCurveLoops(curveLoops);
                foreach (IList<CurveLoop> curveLoops2 in curveLoopLoop)
                {
                    foreach (CurveLoop curveLoop2 in curveLoops2)
                    {
                        // Check if curve loop is counter-clockwise.
                        bool isCCW = curveLoop2.IsCounterclockwise(normal);
                        foreach (Curve curve in curveLoop2)
                        {
                            list_Profile_point.Add(curve.GetEndPoint(0));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return list_Profile_point;
        }

        //----------------------------------------------------------
        public static bool String_To_Bool(string data)
        {
            if (data == "True") return true;
            else return false;
        }

        
    }
}
