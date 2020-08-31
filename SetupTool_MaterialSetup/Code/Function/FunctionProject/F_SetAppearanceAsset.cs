using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SetupTool_MaterialSetup.Code.Function.FunctionProject
{
    class F_SetAppearanceAsset
    {
        //----------------------------------------------------------
        public static void set_appearance_asset(Document doc, Material material)
        {
            try
            {
                var asset = new FilteredElementCollector(doc).OfClass(typeof(AppearanceAssetElement)).First(x => x.Name == "Generic");

                AppearanceAssetElement assetElem = asset as AppearanceAssetElement;

                AppearanceAssetElement assetElemNew = assetElem.Duplicate(material.Name + DateTime.Now.ToString("ddMMyyHHmmss"));

                AppearanceAssetEditScope editScope = new AppearanceAssetEditScope(doc);
                Asset editableAsset = editScope.Start(assetElemNew.Id);
                AssetPropertyDoubleArray4d genericDiffuseProperty = editableAsset.FindByName("generic_diffuse") as AssetPropertyDoubleArray4d;
                genericDiffuseProperty.SetValueAsColor(material.Color);
                AssetPropertyDouble genericTransparency = editableAsset.FindByName("generic_transparency") as AssetPropertyDouble;
                genericTransparency.Value = Convert.ToDouble(material.Transparency) / 100;

                editScope.Commit(true);
                material.AppearanceAssetId = assetElemNew.Id;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
