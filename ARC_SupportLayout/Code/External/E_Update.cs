#region Namespaces
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using ARC_SupportLayout.Code.Function;
using ARC_SupportLayout.Data;
using ARC_SupportLayout.Data.Binding;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using MessageBox = System.Windows.MessageBox;
#endregion

namespace ARC_SupportLayout.Code.External
{
    class E_Update : IExternalEventHandler
    {
        public ListView data_width { get; set; }

        public ObservableCollection<data_width> my_width { get; set; }

        public void Execute(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            try
            {
                Transaction transaction = new Transaction(doc);
                transaction.Start("Width");

                update_width(uiapp, doc);

                transaction.Commit();
                F_SetWidth.set_width(doc);
                List<ElementType> data_type = new FilteredElementCollector(doc).WhereElementIsElementType().Cast<ElementType>().ToList();
                F_GetWidth.get_width(my_width, data_width, data_type);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return;
        }
        public string GetName()
        {
            return "Width";
        }

        public Result update_width(UIApplication uiapp, Document doc)
        {
            try
            {
                foreach(data_width data in CollectionViewSource.GetDefaultView(data_width.ItemsSource).Cast<data_width>().ToList())
                {
                    foreach(ElementType type in data.types)
                    {
                        type.LookupParameter(Source.share_para_hoan_thien).SetValueString(data.width);
                    }
                }
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return Result.Failed;
            }
        }
    }
}
