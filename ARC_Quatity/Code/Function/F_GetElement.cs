using ARC_Quatity.Data.Binding;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ARC_Quatity.Code.Function
{
    class F_GetElement
    {
        //doc + my_element_link----------------------------------------------------------
        public static void Get_ELement_Link_Or_NoLink(Document doc, ObservableCollection<data_element_link> my_element_link)
        {
            try
            {
                //List<Element> elements = new List<Element>();
                List<Element> elements = new FilteredElementCollector(doc)
                    .WhereElementIsNotElementType()
                    .Where(x => x.LookupParameter("Volume") != null || x.LookupParameter("Area") != null || x.LookupParameter("Length") != null)
                    .Where(x => x.Category != null && x.Category.CategoryType.ToString() == "Model" && x.Category.AllowsBoundParameters == true)
                    .ToList();

                foreach (Element element in elements)
                {
                    if (element is FamilyInstance)
                    {
                        FamilyInstance familyInstance = element as FamilyInstance;
                        if (familyInstance.SuperComponent == null)
                        {
                            my_element_link.Add(new data_element_link() { cau_kien = element, doc = doc });
                        }
                    }
                    else
                    {
                        my_element_link.Add(new data_element_link() { cau_kien = element, doc = doc });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //uidoc + doc + my_element_link----------------------------------------------------------
        public static void get_element_by_select(UIDocument uidoc, Document doc, ObservableCollection<data_element_link> my_element_link)
        {
            try
            {
                Selection selection = uidoc.Selection;
                ObservableCollection<data_element_link> my_element_link_support = new ObservableCollection<data_element_link>();
                selection.PickObjects(ObjectType.Element).ToList().Select(x => doc.GetElement(x))
                    .Where(x => x.LookupParameter("Volume") != null || x.LookupParameter("Area") != null || x.LookupParameter("Length") != null)
                    .Where(x => x.Category != null && x.Category.CategoryType.ToString() == "Model" && x.Category.AllowsBoundParameters == true)
                    //.Where(x => x.LookupParameter("Volume") != null && x.LookupParameter("Volume").AsDouble() != 0)
                    .ToList().ForEach(item => my_element_link_support.Add(new data_element_link() { cau_kien = item, doc = doc }));
                foreach (data_element_link element in my_element_link_support)
                {
                    Element model = element.cau_kien;

                    if (model is FamilyInstance)
                    {
                        FamilyInstance familyInstance = model as FamilyInstance;
                        if (familyInstance.SuperComponent == null)
                        {
                            my_element_link.Add(new data_element_link() { cau_kien = model, doc = doc });
                        }
                    }
                    else
                    {
                        my_element_link.Add(new data_element_link() { cau_kien = model, doc = doc });
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
