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
using Brush = System.Windows.Media.Brush;
using Image = System.Windows.Controls.Image;
using MessageBox = System.Windows.MessageBox;

namespace SetupTool_TypeSetup
{
    public class FunctionSupoort
    {
        ListSource mySource;
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

                    list_descipline_data = new ObservableCollection<Data>(Get_Data_List(du_lieu, "list_descipline").Select(x => new Data()
                    {
                        single_value = x.Split('\t')[2],
                        ten_type = x.Split('\t')[3]
                    })),

                    list_position_data = new ObservableCollection<list_position>(Get_Data_List(du_lieu, "list_position").Select(x => new list_position()
                    {
                        position_name = x.Split('\t')[2],
                        position_key = x.Split('\t')[3]
                    })),

                    list_category_data = new ObservableCollection<Data>(Get_Data_List(du_lieu, "list_category").Select(x => new Data()
                    {
                        single_value = x.Split('\t')[2],
                        ten_type = x.Split('\t')[3]
                    })),

                    list_builtin_parameter_group_data = new ObservableCollection<BuiltIn_Parameter_Group>(Get_Data_List(du_lieu, "list_builtin_parameter_group").Select(x => new BuiltIn_Parameter_Group()
                    {
                        ten_builtin = x.Split('\t')[2],
                        builtin = (BuiltInParameterGroup)Enum.Parse(typeof(BuiltInParameterGroup), x.Split('\t')[3])
                    })),

                    list_system_name_data = new ObservableCollection<Type_Group>(Get_Data_List(du_lieu, "list_system_name").Select(x => new Type_Group()
                    {
                        ten_type = x.Split('\t')[2]
                    })),

                    list_parameter_share_data = new ObservableCollection<string>(Get_Data_List(du_lieu, "parameter_share").Select(x => x.Split('\t')[2]))
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
        public double GetParameterInformation(Parameter para)
        {
            double defValue = 0;
            // Use different method to get parameter data according to the storage type
            try
            {
                if (para.StorageType == StorageType.Double)
                {
                    defValue = para.AsDouble();
                }
                if (para.StorageType == StorageType.Integer)
                {
                    defValue = Convert.ToInt32(para.AsInteger());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return defValue;
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
        public string Create_Preview(ElementType type, All_Data myAll_Data)
        {
            string path = @"D:\00 CuaKhanh\03 Revit\00 Ribbon_Mr.W\Image\Logo\Logo1.png";
            try
            {
                mySource = new ListSource();

                if (Directory.Exists(mySource.user + myAll_Data.list_path_foder_data[1]) == false) Directory.CreateDirectory(mySource.user + myAll_Data.list_path_foder_data[1]);

                System.Drawing.Size imgSize = new System.Drawing.Size(200, 200);

                Bitmap image = type.GetPreviewImage(imgSize);
                image.MakeTransparent(System.Drawing.Color.FromArgb(74, 85, 91));

                // encode image to jpeg for test display purposes:

                JpegBitmapEncoder encoder
                  = new JpegBitmapEncoder();

                encoder.Frames.Add(BitmapFrame.Create(ConvertBitmapToBitmapSource(image)));

                encoder.QualityLevel = 25;

                path = mySource.user + myAll_Data.list_path_foder_data[1] + "\\" + type.FamilyName + ".jpg";

                FileStream file = new FileStream(path, FileMode.Create, FileAccess.Write);

                encoder.Save(file);
                file.Close();
            }
            catch (Exception)
            {

            }
            return path;
        }

        //----------------------------------------------------------
        public void View_Dimensions_FamilySymbol(Document doc, ElementType type, ListView list_view, ObservableCollection<Parameters_Family> myParameters_Family, All_Data myAll_Data)
        {
            try
            {
                mySource = new ListSource();
               
                foreach (Parameter para in type.Parameters)
                {
                    try
                    {
                        if (para.Definition.ParameterGroup == myAll_Data.list_builtin_parameter_group_data[0].builtin && para.IsReadOnly == false)
                        {
                            string value = "0";
                            if (para.DisplayUnitType.ToString() == "DUT_DECIMAL_DEGREES")
                            {
                                value = Math.Round(GetParameterInformation(para) * myAll_Data.list_unit_value_data[5], 0).ToString();
                            }
                            else
                            {
                                if (para.StorageType == StorageType.Integer)
                                {
                                    value = para.AsInteger().ToString();
                                }
                                else
                                {
                                    value = Math.Round(GetParameterInformation(para) * myAll_Data.list_unit_value_data[1], 0).ToString();
                                }
                            }
                            myParameters_Family.Add(new Parameters_Family()
                            {
                                ten_parameter = para.Definition.Name,
                                gia_tri_parameter = value,
                                group_parameter = myAll_Data.list_builtin_parameter_group_data[0].ten_builtin,
                                parameter = para
                            });
                        }
                        else if (para.Definition.ParameterGroup == myAll_Data.list_builtin_parameter_group_data[1].builtin && para.IsReadOnly == false)
                        {
                            string value = "0";
                            if (para.DisplayUnitType.ToString() == "DUT_DECIMAL_DEGREES")
                            {
                                value = Math.Round(GetParameterInformation(para) * myAll_Data.list_unit_value_data[5], 0).ToString();
                            }
                            else
                            {
                                value = Math.Round(GetParameterInformation(para) * myAll_Data.list_unit_value_data[1], 0).ToString();
                            }
                            myParameters_Family.Add(new Parameters_Family()
                            {
                                ten_parameter = para.Definition.Name,
                                gia_tri_parameter = value,
                                group_parameter = myAll_Data.list_builtin_parameter_group_data[1].ten_builtin,
                                parameter = para
                            });
                        }
                        else if (para.Definition.ParameterGroup == myAll_Data.list_builtin_parameter_group_data[3].builtin && para.IsReadOnly == false)
                        {
                            string value = GetParameterInformation(para).ToString();
                            myParameters_Family.Add(new Parameters_Family()
                            {
                                ten_parameter = para.Definition.Name,
                                gia_tri_parameter = value,
                                group_parameter = myAll_Data.list_builtin_parameter_group_data[3].ten_builtin,
                                parameter = para
                            });
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
                
                var a = myParameters_Family.OrderBy(x => x.ten_parameter).ToList();
                myParameters_Family.Clear();
                foreach (var b in a)
                {
                    myParameters_Family.Add(b);
                }

                list_view.ItemsSource = myParameters_Family;

                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(list_view.ItemsSource);
                PropertyGroupDescription groupDescription1 = new PropertyGroupDescription("group_parameter");
                view.GroupDescriptions.Add(groupDescription1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void View_Material_FamilySymbol(Document doc, ElementType type, ListView list_view, ObservableCollection<Material_Family> myMaterial_Family, ObservableCollection<Data> materials, All_Data myAll_Data)
        {
            try
            {
                mySource = new ListSource();
                List<Parameter> materialDefinition = new List<Parameter>();
                List<string> materialDefinitionName = new List<string>();
                foreach (Parameter para in type.Parameters)
                {
                    if (para.Definition.ParameterGroup == myAll_Data.list_builtin_parameter_group_data[2].builtin)
                    {
                        string ten = "<By Category>";
                        string ma = "";
                        if (para.AsElementId().IntegerValue != -1)
                        {
                            ten = doc.GetElement(para.AsElementId()).Name;
                            ma =  Check_Para_And_Get_Para(doc.GetElement(para.AsElementId()) as Material, myAll_Data.list_material_para_data[0].material_para_guid, myAll_Data.list_material_para_data[0].material_para_name);
                        }
                        myMaterial_Family.Add(new Material_Family()
                        {
                            ten_cong_tac = para.Definition.Name,
                            ten_vat_lieu_list = materials,
                            ten_vat_lieu = materials.First(x => x.single_value == ten),
                            parameter = para
                        });
                    }
                }
                list_view.ItemsSource = myMaterial_Family;

                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(list_view.ItemsSource);

                // sort list view
                view.SortDescriptions.Add(new SortDescription("ten_cong_tac", ListSortDirection.Ascending));

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void View_Dimensions_And_Material_System(Document doc, ElementType type, ListView list_view_kich_thuoc, ObservableCollection<Parameters_Family> myParameters_Family,
                                                                                               ListView list_view_cong_tac, ObservableCollection<Material_Family> myMaterial_Family, ObservableCollection<Data> materials, All_Data myAll_Data)
        {
            try
            {
                mySource = new ListSource();

                var TypeOftype = type.GetType();
                CompoundStructure compound = null;
                if (TypeOftype.Name == myAll_Data.list_system_name_data[0].ten_type)
                {
                    var wall = type as WallType;
                    compound = wall.GetCompoundStructure();
                }
                if (TypeOftype.Name == myAll_Data.list_system_name_data[1].ten_type)
                {
                    var wall = type as FloorType;
                    compound = wall.GetCompoundStructure();
                }
                if (TypeOftype.Name == myAll_Data.list_system_name_data[2].ten_type)
                {
                    var wall = type as RoofType;
                    compound = wall.GetCompoundStructure();
                }
                if (TypeOftype.Name == myAll_Data.list_system_name_data[3].ten_type)
                {
                    var wall = type as CeilingType;
                    compound = wall.GetCompoundStructure();
                }


                IList<CompoundStructureLayer> getlayer = compound.GetLayers();
                foreach (var layer in getlayer)
                {
                    myParameters_Family.Add(new Parameters_Family()
                    {
                        ten_parameter = layer.Function.ToString(),
                        gia_tri_parameter = Math.Round(layer.Width * myAll_Data.list_unit_value_data[1], 0).ToString(),
                        group_parameter = myAll_Data.list_builtin_parameter_group_data[0].ten_builtin,
                        layer = layer,
                        compound = compound
                    });
                }

                list_view_kich_thuoc.ItemsSource = myParameters_Family;

                CollectionView view_kich_thuoc = (CollectionView)CollectionViewSource.GetDefaultView(list_view_kich_thuoc.ItemsSource);
                PropertyGroupDescription groupDescription1 = new PropertyGroupDescription("group_parameter");
                view_kich_thuoc.GroupDescriptions.Add(groupDescription1);
                //---------------------------------------------------------------------------------------------------------------------
                foreach (var layer in getlayer)
                {
                    string ten = "<By Category>";
                    string ma = "";
                    if (layer.MaterialId.IntegerValue != -1)
                    {
                        ten = doc.GetElement(layer.MaterialId).Name;
                        ma = Check_Para_And_Get_Para(doc.GetElement(layer.MaterialId) as Material, myAll_Data.list_material_para_data[0].material_para_guid, myAll_Data.list_material_para_data[0].material_para_name);
                    }
                    myMaterial_Family.Add(new Material_Family()
                    {
                        ten_cong_tac = layer.Function.ToString(),
                        ten_vat_lieu_list = materials,
                        ten_vat_lieu = materials.First(x => x.single_value == ten),
                        layer = layer,
                        compound = compound
                    });
                }
                list_view_cong_tac.ItemsSource = myMaterial_Family;
            }
            catch (Exception)
            {
                list_view_kich_thuoc.ItemsSource = new ObservableCollection<Parameters_Family>();
                list_view_cong_tac.ItemsSource = new ObservableCollection<Material_Family>();
            }
        }

        //----------------------------------------------------------
        public CompoundStructure Set_Compound(CompoundStructure compound, ObservableCollection<Material_Family> myMaterial_Family, ObservableCollection<Parameters_Family> myParameters_Family, All_Data myAll_Data)
        {
            CompoundStructure compound_new = compound;
            try
            {
                mySource = new ListSource();
                IList<CompoundStructureLayer> cslayers = compound_new.GetLayers();
                ElementId id = new ElementId(-1);

                for (int i = 0; i < cslayers.Count; i++)
                {
                    compound_new.SetLayerWidth(myParameters_Family[i].layer.LayerId, Convert.ToDouble(myParameters_Family[i].gia_tri_parameter) / myAll_Data.list_unit_value_data[1]);
                    if (myMaterial_Family[i].ten_vat_lieu.single_value != "<By Category>")
                    {
                        compound_new.SetMaterialId(myMaterial_Family[i].layer.LayerId, myMaterial_Family[i].ten_vat_lieu.vat_lieu.Id);
                    }
                    else
                    {
                        compound_new.SetMaterialId(myMaterial_Family[i].layer.LayerId, id);
                    }
                }
            }
            catch (Exception)
            {

            }
            return compound_new;
        }

        //----------------------------------------------------------
        public ElementType Set_Type(CompoundStructure compound, ObservableCollection<Material_Family> myMaterial_Family, ObservableCollection<Parameters_Family> myParameters_Family, ElementType elementType, string CreateOrUpdate, string name_type_new, All_Data myAll_Data)
        {
            ElementType elementType_new = null;
            try
            {
                mySource = new ListSource();
                
                var TypeOftype = elementType.GetType();
                if (CreateOrUpdate == mySource.Update)
                {
                    Set_Compound(compound, myMaterial_Family, myParameters_Family, myAll_Data);

                    if (elementType.GetType().Name == myAll_Data.list_system_name_data[0].ten_type)
                    {
                        var wall_Type = elementType as WallType;
                        wall_Type.SetCompoundStructure(compound);
                    }
                    if (elementType.GetType().Name == myAll_Data.list_system_name_data[1].ten_type)
                    {
                        var floor_Type = elementType as FloorType;
                        floor_Type.SetCompoundStructure(compound);
                    }
                    if (elementType.GetType().Name == myAll_Data.list_system_name_data[2].ten_type)
                    {
                        var roof_Type = elementType as RoofType;
                        roof_Type.SetCompoundStructure(compound);
                    }
                    if (elementType.GetType().Name == myAll_Data.list_system_name_data[3].ten_type)
                    {
                        var ceiling_Type = elementType as CeilingType;
                        ceiling_Type.SetCompoundStructure(compound);
                    }
                }
                else if (CreateOrUpdate == mySource.Duplicate)
                {
                    if (elementType.GetType().Name == myAll_Data.list_system_name_data[0].ten_type)
                    {
                        var wall_Type = elementType.Duplicate(name_type_new) as WallType;
                        elementType_new = wall_Type;
                        wall_Type.SetCompoundStructure(Set_Compound(wall_Type.GetCompoundStructure(), myMaterial_Family, myParameters_Family, myAll_Data));
                    }
                    if (elementType.GetType().Name == myAll_Data.list_system_name_data[1].ten_type)
                    {
                        var floor_Type = elementType.Duplicate(name_type_new) as FloorType;
                        elementType_new = floor_Type;
                        floor_Type.SetCompoundStructure(Set_Compound(floor_Type.GetCompoundStructure(), myMaterial_Family, myParameters_Family, myAll_Data));
                    }
                    if (elementType.GetType().Name == myAll_Data.list_system_name_data[2].ten_type)
                    {
                        var roof_Type = elementType.Duplicate(name_type_new) as RoofType;
                        elementType_new = roof_Type;
                        roof_Type.SetCompoundStructure(Set_Compound(roof_Type.GetCompoundStructure(), myMaterial_Family, myParameters_Family, myAll_Data));
                    }
                    if (elementType.GetType().Name == myAll_Data.list_system_name_data[3].ten_type)
                    {
                        var ceiling_Type = elementType.Duplicate(name_type_new) as CeilingType;
                        elementType_new = ceiling_Type;
                        ceiling_Type.SetCompoundStructure(Set_Compound(ceiling_Type.GetCompoundStructure(), myMaterial_Family, myParameters_Family, myAll_Data));
                    }
                }
            }
            catch (Exception)
            {

            }
            return elementType_new;
        }
    }
}
