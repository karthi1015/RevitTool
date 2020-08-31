#region Namespaces
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ListView = System.Windows.Controls.ListView;
using TextBox = System.Windows.Controls.TextBox;
using ComboBox = System.Windows.Controls.ComboBox;
using RadioButton = System.Windows.Controls.RadioButton;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using System.Collections.ObjectModel;
using System.Linq;
using View = Autodesk.Revit.DB.View;
using System.IO;
using WEB_SaveAs.Data.Binding;
using WEB_SaveAs.Binding;
#endregion

namespace WEB_SaveAs
{
    class E_SaveAs : IExternalEventHandler
    {
        public ObservableCollection<data_file> my_data_file { get; set; }

        public ComboBox level { get; set; }

        public TextBox path { get; set; }

        public TextBox name { get; set; }

        public RadioButton option_normal { get; set; }

        public ListView thong_tin_file { get; set; }

        public void Execute(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;
            try
            {
                string result = "F";
                TransactionGroup tr = new TransactionGroup(doc);
                tr.Start("Save As");

                result = Save_As(uiapp, doc);

                tr.Assimilate();
                if (result == "S")
                {
                    doc.Save();
                    string pa = doc.PathName;
                    File.Copy(pa, path.Text + "\\" + name.Text + ".rvt", true);
                    RevitCommandId saveDoc = RevitCommandId.LookupPostableCommandId(PostableCommand.Undo);
                    uiapp.PostCommand(saveDoc);
                    Data_File(path.Text + "\\" + name.Text + ".rvt", doc);
                    MessageBox.Show("Save As Success!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return;
        }
        public string GetName()
        {
            return "SaveAs";
        }

        //----------------------------------------------------------
        public void Data_File(string path, Document doc)
        {
            try
            {
                FileInfo infor = new FileInfo(path);
                List<Level> list_level = new FilteredElementCollector(doc).OfClass(typeof(Level)).Cast<Level>().ToList();
                data_by_level item = (data_by_level)level.SelectedItem;
                double elevation = 1000000000;
                try
                {
                    elevation = list_level.Where(x => Support.RemoveUnicode(x.Name) == item.level).First().Elevation;
                }
                catch (Exception)
                {

                }
                if (my_data_file.Any(x => x.path == path) == false)
                {
                    my_data_file.Add(new data_file()
                    {
                        path = path,
                        name = infor.Name,
                        size = Math.Round(Convert.ToDouble(infor.Length / (1024)), 2).ToString() + " Kb",
                        elevation = elevation
                    });
                }
                thong_tin_file.Items.Refresh();
            }
            catch (Exception)
            {

            }
        }

        //----------------------------------------------------------
        public string Save_As(UIApplication uiapp, Document doc)
        {
            string result = "F";
            try
            {
                if (doc.ActiveView.Name == "{3D}" && doc.ActiveView.ViewType == ViewType.ThreeD)
                {
                    ActiveView3D(uiapp, doc);

                    DeleteView(uiapp, doc);

                    HideAnnotation(uiapp, doc);

                    ClearElement(uiapp, doc);

                    result = "S";
                }
                else
                {
                    MessageBox.Show("Vui lòng mở {3D} view và thử lại lần nữa!", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);

                    result = "F";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }

        //----------------------------------------------------------
        public void ActiveView3D(UIApplication uiapp, Document doc)
        {
            try
            {
                View3D view3D = null;
                Transaction tr = new Transaction(doc);
                tr.Start("CreateAndActiveView3D");

                foreach (View view in new FilteredElementCollector(doc).OfClass(typeof(View)).ToList())
                {
                    if (view.Name == "{3D}")
                    {
                        view3D = view as View3D;
                    }
                }
                view3D.DisplayStyle = DisplayStyle.Shading;
                view3D.DetailLevel = ViewDetailLevel.Fine;
                view3D.get_Parameter(BuiltInParameter.VIEW_PHASE_FILTER).Set(new ElementId(-1));
                tr.Commit();

                uiapp.ActiveUIDocument.ActiveView = view3D;
            }
            catch (Exception)
            {

            }
        }

        //----------------------------------------------------------
        public void DeleteView(UIApplication uiapp, Document doc)
        {
            try
            {
                Transaction tr = new Transaction(doc);
                tr.Start("DeleteView");
                var listLink1 = new FilteredElementCollector(doc).OfClass(typeof(CADLinkType)).ToElementIds();
                var listLink2 = new FilteredElementCollector(doc).OfClass(typeof(RevitLinkType)).ToElementIds();
                var listLink3 = new FilteredElementCollector(doc).OfClass(typeof(View)).Cast<View>().Where(x => DocumentValidation.CanDeleteElement(doc, x.Id)).Select(x => x.Id).ToList();
                doc.Delete(listLink1);
                doc.Delete(listLink2);
                doc.Delete(listLink3);
                tr.Commit();
            }
            catch (Exception)
            {

            }
        }

        //----------------------------------------------------------
        public void HideAnnotation(UIApplication uiapp, Document doc)
        {
            try
            {
                Transaction tr = new Transaction(doc);
                tr.Start("HideAnnotation");
                doc.ActiveView.AreAnnotationCategoriesHidden = true;
                doc.ActiveView.AreAnalyticalModelCategoriesHidden = true;
                tr.Commit();
            }
            catch (Exception)
            {

            }
        }

        //----------------------------------------------------------
        public void ClearElement(UIApplication uiapp, Document doc)
        {
            try
            {
                Transaction transaction = new Transaction(doc);
                transaction.Start("ClearElement");
                try
                {
                    List<ElementId> idDelete = new List<ElementId>();

                    if (option_normal.IsChecked == false)
                    {
                        data_by_level item = (data_by_level)level.SelectedItem;

                        foreach (var element in item.elements)
                        {
                            if (DocumentValidation.CanDeleteElement(doc, element.Id))
                            {
                                idDelete.Add(element.Id);
                            }
                        }
                        doc.Delete(idDelete);
                    }

                    PurgeUnusedFamily(doc);
                    PurgeUnusedMaterial(doc);

                    ChangeWallType(uiapp, doc);
                    ChangeFramingAndColumns(uiapp, doc);
                    RemoveFormwork(uiapp, doc);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    transaction.Dispose();
                }

                PurgeUnusedType(doc);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        public void ChangeWallType(UIApplication uiapp, Document doc)
        {
            try
            {
                List<string> modelWallType = new List<string>();
                List<string> qualityWallType = new List<string>();
                var wallType = new FilteredElementCollector(doc).OfClass(typeof(WallType)).ToElements();
                foreach (WallType type in wallType)
                {
                    if (type.Name.Split('_').Count() > 0 && type.Name.Split('_')[0] == "S")
                    {
                        try
                        {
                            CompoundStructure compound = null;
                            compound = type.GetCompoundStructure();
                            IList<CompoundStructureLayer> getlayer = compound.GetLayers();
                            ElementId materialId = new ElementId(-1);
                            double thickness = 0;
                            foreach (var layer in getlayer)
                            {
                                if (layer.Function == MaterialFunctionAssignment.Structure)
                                {
                                    materialId = layer.MaterialId;
                                    thickness = layer.Width;
                                }
                            }
                            //-----------------------------------------------------------------------
                            var countLayer = compound.LayerCount;
                            var layers = compound.GetLayers();
                            for (var i = 0; i < countLayer; i++)
                            {
                                layers.RemoveAt(0);
                            }
                            layers.Add(new CompoundStructureLayer(thickness, MaterialFunctionAssignment.Structure, materialId));
                            compound.SetLayers(layers);
                            type.SetCompoundStructure(compound);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public void ChangeFramingAndColumns(UIApplication uiapp, Document doc)
        {
            try
            {
                var listSymbol = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).ToElements();

                foreach (FamilySymbol symbol in listSymbol)
                {
                    if (symbol.Name.Split('_').Count() > 0 && symbol.Name.Split('_')[0] == "S")
                    {
                        try
                        {
                            if (symbol.LookupParameter("Ván khuôn") != null)
                            {
                                symbol.LookupParameter("Ván khuôn").Set(0);
                            }
                            if (Source.visible_false.Exists(x => x == symbol.Name.Split('_')[2]))
                            {
                                foreach (Parameter para in symbol.Parameters)
                                {
                                    if (para.Definition.ParameterGroup == BuiltInParameterGroup.PG_VISIBILITY && para.IsReadOnly == false)
                                    {
                                        para.Set(0);
                                    }
                                }
                                if (symbol.LookupParameter("Khối lượng") != null)
                                {
                                    symbol.LookupParameter("Khối lượng").Set(0);
                                }
                                if (symbol.LookupParameter("0. Cọc tận dụng làm cột") != null)
                                {
                                    symbol.LookupParameter("0. Cọc tận dụng làm cột").Set(1);
                                }
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        public void RemoveFormwork(UIApplication uiapp, Document doc)
        {
            try
            {
                var listMaterial = new FilteredElementCollector(doc).OfClass(typeof(Material)).ToElements();
                List<ElementId> listDelete = new List<ElementId>();
                foreach (Material material in listMaterial)
                {
                    if (material.Name.Contains("Ván khuôn"))
                    {
                        listDelete.Add(material.Id);
                    }
                }
                doc.Delete(listDelete);
            }
            catch (Exception)
            {

            }
        }

        //----------------------------------------------------------
        public void PurgeUnusedFamily(Document doc)
        {
            var allElementsInView = new FilteredElementCollector(doc, doc.ActiveView.Id).WhereElementIsNotElementType().ToElements();
            List<string> familysName = new List<string>();
            foreach (Element ele in allElementsInView)
            {
                string familyName = ele.get_Parameter(BuiltInParameter.ELEM_FAMILY_PARAM).AsValueString();
                familysName.Add(familyName);
            }

            List<string> familysName_Unique = familysName.Distinct().ToList();
            List<string> familysName1 = new List<string>();

            var family = new FilteredElementCollector(doc).OfClass(typeof(Family)).ToList();
            List<ElementId> idDelete = new List<ElementId>();
            foreach (Family fa in family)
            {
                if (DocumentValidation.CanDeleteElement(doc, fa.Id) && familysName_Unique.Contains(fa.Name) == false)
                {
                    idDelete.Add(fa.Id);
                }
            }
            doc.Delete(idDelete);
        }

        public void PurgeUnusedMaterial(Document doc)
        {
            var allElementsInView = new FilteredElementCollector(doc, doc.ActiveView.Id).ToElements();
            List<string> materialsName = new List<string>();
            foreach (Element ele in allElementsInView)
            {
                ICollection<ElementId> IdMaterial = ele.GetMaterialIds(false);
                ICollection<ElementId> IdMaterialPaint = ele.GetMaterialIds(true);
                foreach (ElementId id in IdMaterial)
                {
                    if (id.ToString().Length > 5)
                    {
                        materialsName.Add(doc.GetElement(id).Name);
                    }
                }
                foreach (ElementId id in IdMaterialPaint)
                {
                    if (id.ToString().Length > 5)
                    {
                        materialsName.Add(doc.GetElement(id).Name);
                    }
                }
            }

            List<string> materialsName_Unique = materialsName.Distinct().ToList();

            var materials = new FilteredElementCollector(doc).OfClass(typeof(Material)).ToList();
            List<ElementId> idDelete = new List<ElementId>();
            foreach (Material material in materials)
            {
                if (materialsName_Unique.Contains(material.Name) == false)
                {
                    idDelete.Add(material.Id);
                }
            }
            doc.Delete(idDelete);
        }

        //----------------------------------------------------------
        public void PurgeUnusedType(Document doc)
        {

            var allElementsInView = new FilteredElementCollector(doc, doc.ActiveView.Id).ToElements();
            List<string> typesName = new List<string>();
            List<string> familysName = new List<string>();
            foreach (Element ele in allElementsInView)
            {
                typesName.Add(ele.Name);
                string familyName = ele.get_Parameter(BuiltInParameter.ELEM_FAMILY_PARAM).AsValueString();
                familysName.Add(familyName);
            }

            List<string> typesName_Unique = typesName.Distinct().ToList();
            List<string> familysName_Unique = familysName.Distinct().ToList();

            var elementType = new FilteredElementCollector(doc).OfClass(typeof(ElementType)).ToList();
            var elementType1 = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).ToList();
            List<ElementId> idDelete = new List<ElementId>();
            foreach (ElementType type in elementType)
            {
                if (DocumentValidation.CanDeleteElement(doc, type.Id) && typesName_Unique.Contains(type.Name) == false && familysName_Unique.Contains(type.FamilyName))
                {
                    idDelete.Add(type.Id);
                }
            }
            foreach (FamilySymbol type in elementType1)
            {
                if (DocumentValidation.CanDeleteElement(doc, type.Id) && typesName_Unique.Contains(type.Name) == false && familysName_Unique.Contains(type.FamilyName))
                {
                    idDelete.Add(type.Id);
                }
            }
            foreach (ElementId type in idDelete)
            {
                Transaction transaction = new Transaction(doc);
                FailureHandlingOptions failure = transaction.GetFailureHandlingOptions();
                failure.SetClearAfterRollback(true);
                failure.SetForcedModalHandling(false);
                failure.SetFailuresPreprocessor(new RollbackIfErrorOccurs());
                transaction.SetFailureHandlingOptions(failure);
                transaction.Start("PurgeUnusedType");
                try
                {
                    doc.Delete(type);
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Dispose();
                }
            }
        }
    }
    public class RollbackIfErrorOccurs : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            // if there are any failures, rollback the transaction
            if (failuresAccessor.GetFailureMessages().Count > 0)
            {
                return FailureProcessingResult.ProceedWithRollBack;
            }
            else
            {
                return FailureProcessingResult.Continue;
            }
        }
    }
}
