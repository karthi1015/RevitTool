using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;
using WEB_SaveAs.Data.Binding;
using ListView = System.Windows.Controls.ListView;
using TextBox = System.Windows.Controls.TextBox;

namespace WEB_SaveAs.Code.Function
{
    class F_Folder
    {
        //----------------------------------------------------------
        public static void select_folder(Document doc, ObservableCollection<data_file> my_data_file, TextBox folder, ListView thong_tin_file)
        {
            try
            {
                FolderBrowserDialog folderBrowser = new FolderBrowserDialog();
                if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    folder.Text = folderBrowser.SelectedPath;
                }
                get_file_data(doc, my_data_file, folder, thong_tin_file);
            }
            catch (Exception)
            {

            }
        }

        //----------------------------------------------------------
        public static void get_file_data(Document doc,ObservableCollection<data_file> my_data_file, TextBox folder, ListView thong_tin_file)
        {
            try
            {
                my_data_file = new ObservableCollection<data_file>();
                List<Level> list_level = new FilteredElementCollector(doc).OfClass(typeof(Level)).Cast<Level>().ToList();
                foreach (string file in Directory.GetFiles(folder.Text, "*.rvt*").ToList())
                {
                    FileInfo infor = new FileInfo(file);
                    double elevation = 1000000000;
                    try
                    {
                        elevation = list_level.Where(x => Support.RemoveUnicode(x.Name) == infor.Name.Split('_')[1]).First().Elevation;
                    }
                    catch (Exception)
                    {

                    }
                    my_data_file.Add(new data_file()
                    {
                        path = file,
                        name = infor.Name,
                        size = Math.Round(Convert.ToDouble(infor.Length / (1024)), 2).ToString() + " Kb",
                        elevation = elevation
                    });
                }
                thong_tin_file.ItemsSource = my_data_file;

                thong_tin_file.Items.SortDescriptions.Add(new SortDescription("elevation", ListSortDirection.Descending));
                thong_tin_file.Items.SortDescriptions.Add(new SortDescription("name", ListSortDirection.Descending));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
