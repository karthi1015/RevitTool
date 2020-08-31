using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WEB_SaveAs.Binding;
using WEB_SaveAs.Data.Binding;

namespace WEB_SaveAs.Code.Function
{
    class F_ElementByLevel
    {
        //----------------------------------------------------------
        public static void get_element_by_level(Document doc, List<Level> level_list, ComboBox level_combobox)
        {
            try
            {
                level_list = new FilteredElementCollector(doc).OfClass(typeof(Level)).Cast<Level>().ToList();
                var list_element = new FilteredElementCollector(doc)
                    .WhereElementIsNotElementType()
                    .Where(x => x.Parameters.Cast<Parameter>().Any(y => y.Definition.Name == "Volume" || y.Definition.Name == "Area" || y.Definition.Name == "Length") == true)
                    .Where(x => x.Category != null && x.Category.CategoryType.ToString() == "Model" && x.Category.AllowsBoundParameters == true)
                    .ToList();

                List<data_element_level> myelement_list = new List<data_element_level>();
                foreach (Element element in list_element)
                {
                    string block = doc.ProjectInformation.BuildingName;

                    string level = "";
                    if (element.LookupParameter(Source.share_para_text3).AsString() != null) level = element.LookupParameter(Source.share_para_text3).AsString();

                    double elevation = 100000000;
                    try
                    {
                        elevation = level_list.First(x => level == x.Name).Elevation;
                    }
                    catch (Exception)
                    {

                    }
                    myelement_list.Add(new data_element_level
                    {
                        cau_kien = element,
                        level = Support.RemoveUnicode(level),
                        elevation = elevation
                    });
                }

                List<data_by_level> my_level_block = new List<data_by_level>(myelement_list.GroupBy(x => new
                {
                    x.level,
                    x.elevation
                }).Where(x => !string.IsNullOrEmpty(x.Key.level)).Select(y => new data_by_level()
                {
                    number = doc.ProjectInformation.Number,
                    block = doc.ProjectInformation.BuildingName,
                    level = y.Key.level,
                    level_file = doc.Title.Split('_')[2],
                    descipline = doc.Title.Split('_')[3],
                    elevation = y.Key.elevation
                }).OrderByDescending(z => z.elevation));

                level_combobox.ItemsSource = my_level_block;
                level_combobox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {

            }
        }
    }
}
