using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tool_CheckUpdateModel.Data;
using Tool_CheckUpdateModel.Data.Binding;
using ComboBox = System.Windows.Controls.ComboBox;

namespace Tool_CheckUpdateModel.Function
{
    class E_LoadForm : IExternalEventHandler
    {
        public string path { get; set; }
        public ComboBox link_file { get; set; }

        public void Execute(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            Transaction transaction = new Transaction(doc);
            transaction.Start("LoadForm");

            Load_Form(uiapp, doc);

            transaction.Commit();
        }

        public string GetName()
        {
            return "LoadForm";
        }

        public void Load_Form(UIApplication uiapp, Document doc)
        {
            try
            {
                data_revit_link item = (data_revit_link)link_file.SelectedItem;
                doc.Delete(item.type.Id);

                ModelPath mp = ModelPathUtils.ConvertUserVisiblePathToModelPath(path);
                RevitLinkOptions rlo = new RevitLinkOptions(false);
                var linkType = RevitLinkType.Create(doc, mp, rlo);
                var instance = RevitLinkInstance.Create(doc, linkType.ElementId);

                List<Document> docs = new List<Document>();
                foreach (Document d in uiapp.Application.Documents)
                {
                    docs.Add(d);
                }
                item.document = docs.First(y => y.Title + ".rvt" == item.name);
                item.type = doc.GetElement(linkType.ElementId) as RevitLinkType;
                link_file.Items.Refresh();
                MessageBox.Show(item.document.PathName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                link_file.SelectedItem = null;
            }
        }
    }
}
