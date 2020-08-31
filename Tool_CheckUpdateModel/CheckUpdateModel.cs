#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.ExtensibleStorage;
using Autodesk.Revit.DB.Visual;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Tool_CheckUpdateModel.Data;
using Application = Autodesk.Revit.ApplicationServices.Application;
#endregion

namespace Tool_CheckUpdateModel
{
    [Transaction(TransactionMode.Manual)]
    public class CheckUpdateModel : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            HwndSource hwndSource = HwndSource.FromHwnd(uiapp.MainWindowHandle);
            Window wnd = hwndSource.RootVisual as Window;

            Selection select = uidoc.Selection;

            try
            {
                Transaction tr = new Transaction(doc);
                tr.Start("UpdateModel");
                //CheckUpdateModelMain viewWPF = new CheckUpdateModelMain(uiapp);
                //viewWPF.Owner = wnd;
                //viewWPF.Show();

                //Get_Elevation_Ceilings(doc);

                //Change_Data_Material(doc);

                foreach(Material material in new FilteredElementCollector(doc).OfClass(typeof(Material)).Cast<Material>().ToList())
                {
                    SetColorAppearanceAsset(doc, material);
                }

                tr.Commit();

                
            }
            catch (Exception ex)
            {
                message = ex.ToString();
                return Result.Failed;
            }
            return Result.Succeeded;
        }

        //----------------------------------------------------------
        public void SetColorAppearanceAsset(Document doc, Material material)
        {
            try
            {
                var list = new FilteredElementCollector(doc).OfClass(typeof(AppearanceAssetElement)).ToList();
                List<string> name = new List<string>();
                foreach (AppearanceAssetElement e in list)
                {
                    name.Add(e.Name);
                }
                string newName = "";
                for (var i = 0; i < 10000000; i++)
                {
                    if (name.Any(x => x == material.Name + "(" + i.ToString() + ")") == false)
                    {
                        newName = material.Name + "(" + i.ToString() + ")";
                        break;
                    }
                }

                AppearanceAssetElement assetElem = list[0] as AppearanceAssetElement;

                AppearanceAssetElement assetElemNew = assetElem.Duplicate(newName);

                AppearanceAssetEditScope editScope = new AppearanceAssetEditScope(doc);
                Asset editableAsset = editScope.Start(assetElemNew.Id);
                AssetPropertyDoubleArray4d genericDiffuseProperty = editableAsset["generic_diffuse"] as AssetPropertyDoubleArray4d;
                genericDiffuseProperty.SetValueAsColor(material.Color);
                AssetPropertyDouble genericTransparency = editableAsset["generic_transparency"] as AssetPropertyDouble;
                genericTransparency.Value = Convert.ToDouble(material.Transparency) / 100;

                editScope.Commit(true);
                material.AppearanceAssetId = assetElemNew.Id;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public static string PrintOutRevitUnitInfo(UnitType ut, FormatOptions obj, string indent)
        {
            string msg = "";

            msg += string.Format(indent + "\tUnit display: {0} ({1})" + Environment.NewLine, LabelUtils.GetLabelFor(obj.DisplayUnits), obj.DisplayUnits);

            return msg;
        }

        public void Get_Elevation_Ceilings(Document doc)
        {
            try
            {
                foreach (Ceiling ceiling in new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_Ceilings).Cast<Ceiling>().ToList())
                {
                    Level level = (doc.GetElement(ceiling.LevelId) as Level);

                    if(ceiling.LookupParameter("Elevation Ceilings") != null)
                    {
                        ceiling.LookupParameter("Elevation Ceilings").Set(level.Elevation + ceiling.get_Parameter(BuiltInParameter.CEILING_HEIGHTABOVELEVEL_PARAM).AsDouble());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Change_Data_Material(Document doc)
        {
            try
            {
                List<string> MCT = new List<string>() { "MCT", "ADE6BDF4-A578-43F3-AF2F-918ADBF5BF1E" };
                List<string> DV = new List<string>() { "DV", "CC58FBE5-B872-460A-A36B-9889B5FA07AF" };
                List<string> TON = new List<string>() { "TON", "62BD329D-230A-480B-9B60-8782BDD7F42F" };
                List<string> USER = new List<string>() { "USER", "4108B042-364F-4E11-A5A8-EC2ED6A7C3CE" };
                List<string> TIME = new List<string>() { "TIME", "62EB044B-FFE5-456A-96EE-6C3A1E3D300B" };

                foreach (Material material in new FilteredElementCollector(doc).OfClass(typeof(Material)).Cast<Material>().ToList())
                {
                    string keynote = "";
                    string unit = "";
                    if (!string.IsNullOrEmpty(material.get_Parameter(BuiltInParameter.KEYNOTE_PARAM).AsString())) keynote = material.get_Parameter(BuiltInParameter.KEYNOTE_PARAM).AsString();
                    if (!string.IsNullOrEmpty(material.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString())) unit = material.get_Parameter(BuiltInParameter.ALL_MODEL_MARK).AsString();

                    SetDataStorage(material, MCT[1], MCT[0], keynote);
                    SetDataStorage(material, DV[1], DV[0], unit);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void SetDataStorage(Material material, string guid, string name_para, string value)
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
    }
}
