using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.UI;
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
using Brush = System.Windows.Media.Brush;
using Image = System.Windows.Controls.Image;
using MessageBox = System.Windows.MessageBox;
using Size = System.Drawing.Size;

namespace Draw_All
{
    public class Support_All
    {
        ListSource mySource;
        FunctionSQL mySQL;
        //----------------------------------------------------------
        public string Get_Data_String(List<string> list, string key)
        {
            string value = "";
            try
            {
                value = list.First(x => x.Split('\t')[0] == "ALL" && x.Split('\t')[1] == key);
            }
            catch (Exception)
            {

            }
            return value;
        }

        //----------------------------------------------------------
        public List<string> Get_Data_List(List<string> list, string key)
        {
            List<string> value = new List<string>();
            try
            {
                value = list.Where(x => x.Split('\t')[0] == "ALL" && x.Split('\t')[1] == key).ToList();
            }
            catch (Exception)
            {

            }
            return value;
        }
        //----------------------------------------------------------
        public All_Data Get_Data_All(string path)
        {
            All_Data myAll_Data = new All_Data();
            try
            {
                List<string> du_lieu = File.ReadAllLines(path).ToList();

                myAll_Data = new All_Data()
                {
                    list_path_image_data = new ObservableCollection<list_image>(Get_Data_List(du_lieu, "path_image").Select(x => new list_image()
                    {
                        image_name = x.Split('\t')[2],
                        image_path = x.Split('\t')[3]
                    })),

                    list_unit_value_data = new ObservableCollection<double>(Get_Data_List(du_lieu, "unit_value").Select(x => Convert.ToDouble(x.Split('\t')[2]))),

                    list_color_UI_data = new ObservableCollection<Brush>(Get_Data_List(du_lieu, "color_UI").Select(x =>
                    (SolidColorBrush)new BrushConverter().ConvertFromString(x.Split('\t')[2]))),

                    list_procedure_data = new ObservableCollection<list_procedure>(Get_Data_List(du_lieu, "sp_quantity").Select(x => new list_procedure()
                    {
                        procedure_name = x.Split('\t')[2],
                        procedure_para = x.Split('\t')[3]
                    })),

                    list_path_foder_data = new ObservableCollection<string>(Get_Data_List(du_lieu, "path_foder").Select(x => x.Split('\t')[2])),

                    list_path_connect_SQL_data = new ObservableCollection<string>(Get_Data_List(du_lieu, "path_connect_SQL").Select(x => x.Split('\t')[2])),

                    list_material_para_data = new ObservableCollection<list_material_para>(Get_Data_List(du_lieu, "material_parameter").Select(x => new list_material_para()
                    {
                        material_para_guid = x.Split('\t')[2],
                        material_para_name = x.Split('\t')[3]
                    })),

                    list_mct_descipline_data = new ObservableCollection<list_mct_descipline>(Get_Data_List(du_lieu, "mct_descipline").Select(x => new list_mct_descipline()
                    {
                        mct = x.Split('\t')[2],
                        descipline = x.Split('\t')[3]
                    })),

                    list_parameter_share_data = new ObservableCollection<string>(Get_Data_List(du_lieu, "parameter_share").Select(x => x.Split('\t')[2])),

                    list_category_draw = new ObservableCollection<string>(Get_Data_List(du_lieu, "list_category_draw").Select(x => x.Split('\t')[2])),

                    list_category_draw_host = new ObservableCollection<string>(Get_Data_List(du_lieu, "list_category_draw_host").Select(x => x.Split('\t')[2])),

                    list_parameter_tag = new ObservableCollection<string>(Get_Data_List(du_lieu, "list_parameter_tag").Select(x => x.Split('\t')[2])),

                    list_parameter_framing = new ObservableCollection<string>(Get_Data_List(du_lieu, "list_parameter_framing").Select(x => x.Split('\t')[2])),

                    list_parameter_column = new ObservableCollection<string>(Get_Data_List(du_lieu, "list_parameter_column").Select(x => x.Split('\t')[2])),

                    list_parameter_opening = new ObservableCollection<string>(Get_Data_List(du_lieu, "list_parameter_opening").Select(x => x.Split('\t')[2])),

                    list_category_tag = new ObservableCollection<string>(Get_Data_List(du_lieu, "list_category_tag").Select(x => x.Split('\t')[2]))
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return myAll_Data;
        }

        //----------------------------------------------------------
        public void Default_Image(All_Data myAll_Data, List<Image> list_control_image)
        {
            try
            {
                for (int i = 0; i < list_control_image.Count; i++)
                {
                    list_control_image[i].Source = new BitmapImage(new Uri(myAll_Data.list_path_image_data.First(x => x.image_name == list_control_image[i].Name).image_path));
                }
            }
            catch (Exception)
            {

            }
        }

        //----------------------------------------------------------
        public string Check_Para_And_Get_Para(Material material, string guid, string name_para)
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
        public string RemoveUnicode(string text)
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
        static BitmapSource ConvertBitmapToBitmapSource(Bitmap bmp)
        {
            return System.Windows.Interop.Imaging
              .CreateBitmapSourceFromHBitmap(
                bmp.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }

        //----------------------------------------------------------
        public string CreatePreview(ElementType type, All_Data myAll_Data)
        {
            string path = "";
            try
            {
                mySource = new ListSource();
                if (Directory.Exists(mySource.user + myAll_Data.list_path_foder_data[1]) == false) Directory.CreateDirectory(mySource.user + myAll_Data.list_path_foder_data[1]);

                Size imgSize = new Size(200, 200);

                Bitmap image = type.GetPreviewImage(imgSize);

                // encode image to jpeg for test display purposes:

                JpegBitmapEncoder encoder
                  = new JpegBitmapEncoder();

                encoder.Frames.Add(BitmapFrame.Create(
                  ConvertBitmapToBitmapSource(image)));

                encoder.QualityLevel = 25;

                path = mySource.user + myAll_Data.list_path_foder_data[1] + "\\" + type.FamilyName + ".jpg";

                FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write);

                encoder.Save(file);
                file.Close();
            }
            catch (Exception ex)
            {

            }
            return path;
        }

        //----------------------------------------------------------
        public string Get_Parameter_Information(Parameter para, Document doc, All_Data myAll_Data)
        {
            string defValue = string.Empty;
            try
            {
                switch (para.StorageType)
                {
                    case StorageType.Double:
                        if (para.DisplayUnitType.ToString() == "DUT_DECIMAL_DEGREES")
                        {
                            defValue = Math.Round(para.AsDouble() * myAll_Data.list_unit_value_data[5], mySource.lam_tron).ToString();
                        }
                        else
                        {
                            defValue = Math.Round(para.AsDouble() * myAll_Data.list_unit_value_data[2], mySource.lam_tron).ToString();
                        }
                        break;
                    case StorageType.ElementId:
                        ElementId id = para.AsElementId();
                        if (id.IntegerValue >= 0)
                        {
                            defValue = doc.GetElement(id).Name;
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
                        if (para.AsString() != null)
                        {
                            defValue = para.AsString();
                        }
                        else
                        {
                            defValue = "";
                        }
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

        //----------------------------------------------------------
        public string Get_Parameter_Information_Family(FamilyParameter para, Document familyDoc, FamilyManager manager, All_Data myAll_Data)
        {
            string defValue = string.Empty;
            try
            {
                switch (para.StorageType)
                {
                    case StorageType.Double:
                        if (para.DisplayUnitType.ToString() == "DUT_DECIMAL_DEGREES")
                        {
                            defValue = Math.Round((double)manager.CurrentType.AsDouble(para) * myAll_Data.list_unit_value_data[5], mySource.lam_tron).ToString();
                        }
                        else
                        {
                            defValue = Math.Round((double)manager.CurrentType.AsDouble(para) * myAll_Data.list_unit_value_data[2], mySource.lam_tron).ToString();
                        }
                        break;
                    case StorageType.ElementId:
                        ElementId id = manager.CurrentType.AsElementId(para);
                        if (id.IntegerValue >= 0)
                        {
                            defValue = familyDoc.GetElement(id).Name;
                        }
                        else
                        {
                            defValue = id.IntegerValue.ToString();
                        }
                        break;
                    case StorageType.Integer:
                        if (ParameterType.YesNo == para.Definition.ParameterType)
                        {
                            if (manager.CurrentType.AsInteger(para) == 0)
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
                            defValue = manager.CurrentType.AsInteger(para).ToString();
                        }
                        break;
                    case StorageType.String:
                        if (manager.CurrentType.AsString(para) != null)
                        {
                            defValue = manager.CurrentType.AsString(para);
                        }
                        else
                        {
                            defValue = "";
                        }
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

        //----------------------------------------------------------
        public ObservableCollection<parameter_data> View_Dimensions_FamilySymbol(Document doc, Element element, All_Data myAll_Data)
        {
            mySource = new ListSource();
            ObservableCollection<parameter_data> my_parameter_data = new ObservableCollection<parameter_data>();
            try
            {
                try
                {
                    foreach (Parameter para in element.Parameters)
                    {
                        try
                        {
                            if (para.Definition.ParameterGroup == BuiltInParameterGroup.PG_GEOMETRY && mySource.para_name_not_use.Contains(para.Definition.Name) == false && para.IsReadOnly == false)
                            {
                                string value = Get_Parameter_Information(para, doc, myAll_Data);
                                my_parameter_data.Add(new parameter_data()
                                {
                                    parameter_name = para.Definition.Name,
                                    parameter_value = value,
                                    parameter = para
                                });
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Bạn cần có ít nhất 1 đối tượng đã được vẽ để lấy thông tin!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return my_parameter_data;
        }

        //----------------------------------------------------------
        public void Change_Data_Tag(UIApplication uiapp, Document doc, All_Data myAll_Data)
        {
            try
            {
                var builtin_category = Enum.GetValues(typeof(BuiltInCategory)).Cast<BuiltInCategory>().ToList();
                var categories = doc.Settings.Categories;
                List<BuiltInCategory> list_builtin = new List<BuiltInCategory>();
                foreach (string category in myAll_Data.list_category_tag)
                {
                    foreach (BuiltInCategory buildCategory in builtin_category)
                    {
                        try
                        {
                            if (categories.get_Item(buildCategory) != null && categories.get_Item(buildCategory).Name == category)
                            {
                                list_builtin.Add(buildCategory);
                                break;
                            }
                        }
                        catch (Exception) { }
                    }

                }

                ElementMulticategoryFilter filter_category = new ElementMulticategoryFilter(list_builtin);
                List<Element> list_tag = new FilteredElementCollector(doc)
                    .WherePasses(filter_category)
                    .WhereElementIsNotElementType()
                    .ToList();
                foreach (FamilyInstance familyInstance in list_tag)
                {
                    Element host_of_opening = familyInstance.Host;
                    if (host_of_opening != null)
                    {
                        if (host_of_opening.Category.Name == myAll_Data.list_category_draw_host[0])
                        {

                        }
                        else if (host_of_opening.Category.Name == myAll_Data.list_category_draw_host[1])
                        {
                            Level level = doc.GetElement(familyInstance.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsElementId()) as Level;
                            double offset_bottom = level.Elevation + familyInstance.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).AsDouble();
                            if (familyInstance.LookupParameter(myAll_Data.list_parameter_tag[4]) != null) familyInstance.LookupParameter(myAll_Data.list_parameter_tag[4]).Set(offset_bottom);

                            double offset = 0;
                            if (familyInstance.get_Parameter(BuiltInParameter.FAMILY_ROUGH_HEIGHT_PARAM) != null)
                            {
                                offset = familyInstance.get_Parameter(BuiltInParameter.FAMILY_ROUGH_HEIGHT_PARAM).AsDouble();
                            }
                            double offset_uk = offset_bottom + offset;
                            if (familyInstance.LookupParameter(myAll_Data.list_parameter_tag[1]) != null) familyInstance.LookupParameter(myAll_Data.list_parameter_tag[1]).Set(offset_uk);
                            if (familyInstance.LookupParameter(myAll_Data.list_parameter_tag[2]) != null) familyInstance.LookupParameter(myAll_Data.list_parameter_tag[2]).Set(offset_uk);
                            if (familyInstance.LookupParameter(myAll_Data.list_parameter_tag[3]) != null) familyInstance.LookupParameter(myAll_Data.list_parameter_tag[3]).Set(offset_uk);
                        }
                        else
                        {
                            if (familyInstance.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM) != null
                            && familyInstance.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsElementId().IntegerValue != -1)
                            {
                                Level level = doc.GetElement(familyInstance.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsElementId()) as Level;
                                double offset_bottom = level.Elevation + familyInstance.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).AsDouble();

                                if (familyInstance.LookupParameter(myAll_Data.list_parameter_tag[1]) != null) familyInstance.LookupParameter(myAll_Data.list_parameter_tag[1]).Set(offset_bottom);
                                if (familyInstance.LookupParameter(myAll_Data.list_parameter_tag[2]) != null) familyInstance.LookupParameter(myAll_Data.list_parameter_tag[2]).Set(offset_bottom);
                                if (familyInstance.LookupParameter(myAll_Data.list_parameter_tag[3]) != null) familyInstance.LookupParameter(myAll_Data.list_parameter_tag[3]).Set(offset_bottom);
                            }
                            else if (familyInstance.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM) != null
                                && familyInstance.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM).AsElementId().IntegerValue != -1)
                            {
                                Level level = doc.GetElement(familyInstance.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM).AsElementId()) as Level;
                                double offset_bottom = level.Elevation + familyInstance.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).AsDouble();

                                if (familyInstance.LookupParameter(myAll_Data.list_parameter_tag[1]) != null) familyInstance.LookupParameter(myAll_Data.list_parameter_tag[1]).Set(offset_bottom);
                                if (familyInstance.LookupParameter(myAll_Data.list_parameter_tag[2]) != null) familyInstance.LookupParameter(myAll_Data.list_parameter_tag[2]).Set(offset_bottom);
                                if (familyInstance.LookupParameter(myAll_Data.list_parameter_tag[3]) != null) familyInstance.LookupParameter(myAll_Data.list_parameter_tag[3]).Set(offset_bottom);
                            }
                        }
                    }
                    else
                    {
                        if (familyInstance.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM) != null
                            && familyInstance.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsElementId().IntegerValue != -1)
                        {
                            Level level = doc.GetElement(familyInstance.get_Parameter(BuiltInParameter.FAMILY_LEVEL_PARAM).AsElementId()) as Level;
                            double offset_bottom = level.Elevation + familyInstance.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).AsDouble();

                            if (familyInstance.LookupParameter(myAll_Data.list_parameter_tag[1]) != null) familyInstance.LookupParameter(myAll_Data.list_parameter_tag[1]).Set(offset_bottom);
                            if (familyInstance.LookupParameter(myAll_Data.list_parameter_tag[2]) != null) familyInstance.LookupParameter(myAll_Data.list_parameter_tag[2]).Set(offset_bottom);
                            if (familyInstance.LookupParameter(myAll_Data.list_parameter_tag[3]) != null) familyInstance.LookupParameter(myAll_Data.list_parameter_tag[3]).Set(offset_bottom);
                        }
                        else if (familyInstance.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM) != null
                            && familyInstance.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM).AsElementId().IntegerValue != -1)
                        {
                            Level level = doc.GetElement(familyInstance.get_Parameter(BuiltInParameter.INSTANCE_SCHEDULE_ONLY_LEVEL_PARAM).AsElementId()) as Level;
                            double offset_bottom = level.Elevation + familyInstance.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM).AsDouble();

                            if (familyInstance.LookupParameter(myAll_Data.list_parameter_tag[1]) != null) familyInstance.LookupParameter(myAll_Data.list_parameter_tag[1]).Set(offset_bottom);
                            if (familyInstance.LookupParameter(myAll_Data.list_parameter_tag[2]) != null) familyInstance.LookupParameter(myAll_Data.list_parameter_tag[2]).Set(offset_bottom);
                            if (familyInstance.LookupParameter(myAll_Data.list_parameter_tag[3]) != null) familyInstance.LookupParameter(myAll_Data.list_parameter_tag[3]).Set(offset_bottom);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
