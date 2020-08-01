using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ComboBox = System.Windows.Controls.ComboBox;
using Path = System.IO.Path;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Naviswork_ClashComment.Data;
using System.Net;

namespace Naviswork_ClashComment
{
    public partial class UserControl1 : Window
    {
        //----------------------------------------------------------------------------------------------------------------------------------------
        #region Khai bao Instance
        UIApplication uiapp;
        UIDocument uidoc;
        Document doc;
        string project_number = "";
        Document doc_file_1;
        Document doc_file_2;

        E_Focus my_focus;
        ExternalEvent e_focus;
        #endregion

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        #region Khai bao Observablecollection
        ObservableCollection<parent> my_parent { get; set; }
        ObservableCollection<child> my_child { get; set; }
        ObservableCollection<Image_Solution> my_Image { get; set; }
        #endregion

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        public string path = "";
        public UserControl1(UIApplication _uiapp)
        {
            InitializeComponent();
            uiapp = _uiapp;
            uidoc = uiapp.ActiveUIDocument;
            doc = uidoc.Document;
            project_number = doc.ProjectInformation.Number;

            my_focus = new E_Focus();
            e_focus = ExternalEvent.Create(my_focus);

            Get_Data_For_Parent();
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        #region Clash Parent
        public void Get_Data_For_Parent()
        {
            try
            {
                if (!string.IsNullOrEmpty(project_number))
                {
                    List<string> Para2 = new List<string>() { "@DBProjectNumber" };
                    List<string> Para2_Values = new List<string>() { project_number };
                    var listtotal = SQL.SQLRead(SQL.path_connect_SQL_FileStream, "dbo.sp_ReadData_FromClashDetective", Source.type_Procedure, Para2, Para2_Values);
                    ObservableCollection<parent> parent_support = new ObservableCollection<parent>();
                    for (var i = 0; i < listtotal.Rows.Count; i++)
                    {
                        parent_support.Add(new parent()
                        {
                            clash_parent = listtotal.Rows[i]["DisplayNameParent"].ToString(),
                            status_parent = listtotal.Rows[i]["Status"].ToString(),
                            clash_file_name = listtotal.Rows[i]["FilesName"].ToString()
                        });
                    }
                    status_data status_Data = Source.list_status_parent[0];

                    my_parent = new ObservableCollection<parent>(parent_support.GroupBy(x => new
                    {
                        x.clash_parent
                    }).Where(y => y.Select(yy => yy.status_parent).Any(yyy => yyy != Source.list_status_child[2].name))
                    .Select(z => new parent()
                    {
                        clash_parent = z.Key.clash_parent,
                        status_parent = status_Data.name,
                        color = status_Data.color,
                        clash_file_name = z.First().clash_file_name
                    }));

                    thong_tin_clash_parent.ItemsSource = my_parent;
                    CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_clash_parent.ItemsSource);
                    view.SortDescriptions.Add(new SortDescription("clash_parent", ListSortDirection.Ascending));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //---------------------------------------------------------------------------------
        private void Choose_Clash_Parent(object sender, MouseButtonEventArgs e)
        {
            try
            {
                parent item = (parent)thong_tin_clash_parent.SelectedItem;
                if (item != null)
                {
                    List<Document> docs = new List<Document>();
                    foreach (Document doc in uiapp.Application.Documents)
                    {
                        docs.Add(doc);
                    }

                    string file_name_1 = item.clash_file_name.Split(',')[0];
                    string file_name_2 = item.clash_file_name.Split(',')[1];
                    if (docs.Any(x => x.Title.Split('.')[0] == file_name_1) == false || docs.Any(x => x.Title.Split('.')[0] == file_name_2) == false)
                    {
                        MessageBox.Show("File clash không tồn tại trong dự án. Vui lòng link file cần kiểm tra và thử lại lần nữa", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        foreach (RevitLinkType type in new FilteredElementCollector(doc).OfClass(typeof(RevitLinkType)).Cast<RevitLinkType>().ToList())
                        {
                            if (type.Name.Split('.')[0] == file_name_1 || type.Name.Split('.')[0] == file_name_2)
                            {
                                if (type.IsHidden(doc.ActiveView)) doc.ActiveView.UnhideElements(new List<ElementId>() { type.Id });
                            }
                        }
                        doc_file_1 = docs.First(x => x.Title.Split('.')[0] == file_name_1);
                        doc_file_2 = docs.First(x => x.Title.Split('.')[0] == file_name_2);
                        Get_Data_Clash_Child(item.clash_parent, item.clash_file_name);
                        get_count_state();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        #region Clash Child
        public void Get_Data_Clash_Child(string clash_parent, string file_name)
        {
            try
            {
                my_child = new ObservableCollection<child>();

                List<string> Para2 = new List<string>() { "@DBProjectNumber" };
                List<string> Para2_Values = new List<string>() { project_number };
                var listtotal = SQL.SQLRead(SQL.path_connect_SQL_FileStream, "dbo.sp_ReadData_FromClashDetective_ForClash", Source.type_Procedure, Para2, Para2_Values);
                List<string> check_status = new List<string>();
                for (var i = 0; i < listtotal.Rows.Count; i++)
                {
                    if (listtotal.Rows[i]["FilesName"].ToString() == file_name && listtotal.Rows[i]["DisplayNameParent"].ToString() == clash_parent)
                    {
                        status_data status = Source.list_status_child.First(x => x.name == listtotal.Rows[i]["Status"].ToString());

                        string user = "";
                        string message = "";
                        List<ImageSource> imageSource = new List<ImageSource>();

                        List<string> id_message = new List<string>();
                        string Messages = listtotal.Rows[i]["Messages"].ToString();
                        if (Messages != "null" && !string.IsNullOrEmpty(Messages))
                        {
                            List<data_messages> my_data_messages = JsonConvert.DeserializeObject<List<data_messages>>(Messages);
                            foreach (data_messages data in my_data_messages)
                            {
                                if (data.mark == true)
                                {
                                    if (!string.IsNullOrEmpty(data.message))
                                    {
                                        message += "\n" + data.message;
                                    }
                                    id_message.Add(data.id);
                                }
                            }
                        }

                        string Attach = listtotal.Rows[i]["Attach"].ToString();
                        if (Attach != "null" && !string.IsNullOrEmpty(Attach))
                        {
                            List<data_attach> my_data_attach = JsonConvert.DeserializeObject<List<data_attach>>(Attach);
                            foreach (data_attach data in my_data_attach)
                            {
                                if (data.messageId == "solution" || string.Join("|", id_message).Contains(data.messageId))
                                {
                                    foreach (data_image image in data.images)
                                    {
                                        imageSource.Add(Support.Image_Base64(image.src));
                                    }
                                }
                            }
                        }

                        if (!string.IsNullOrEmpty(listtotal.Rows[i]["ApprovedBy"].ToString()))
                        {
                            user = JObject.Parse(listtotal.Rows[i]["ApprovedBy"].ToString()).GetValue("userName").ToString();
                        }
                        bool isEnable = status.isEnable;

                        string hostName = Dns.GetHostName();
                        string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();
                        path = Source.pathUserPassword + "\\" + myIP;
                        if (File.Exists(path))
                        {
                            var infor = File.ReadAllLines(path).ToList();
                            string userId = infor[1];

                            List<string> list_userId_assign = new List<string>();
                            string assign_to = listtotal.Rows[i]["AssignTo"].ToString();
                            if (assign_to != "null" && !string.IsNullOrEmpty(assign_to))
                            {
                                List<data_assign> my_data_assign = JsonConvert.DeserializeObject<List<data_assign>>(assign_to);
                                foreach (data_assign data in my_data_assign)
                                {
                                    list_userId_assign.Add(data.userId);
                                }
                            }
                            if (list_userId_assign.Any(x => x == userId) == false)
                            {
                                if (status.name != Source.list_status_child[4].name) isEnable = false;
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(infor[2])) isEnable = false;
                            }
                        }
                        Func<char, bool> isnumber = ch => char.IsNumber(ch);
                        int index = 0;
                        for (int j = 0; j < listtotal.Rows[i]["DisplayNameChild"].ToString().Length; j++)
                        {
                            if (isnumber(listtotal.Rows[i]["DisplayNameChild"].ToString()[j]))
                            {
                                index = j;
                                break;
                            }
                        }
                        my_child.Add(new child()
                        {
                            clash_child_sort = Convert.ToInt32(listtotal.Rows[i]["DisplayNameChild"].ToString().Substring(index)),
                            clash_child = listtotal.Rows[i]["DisplayNameChild"].ToString(),
                            status_child_list = Source.list_status_child,
                            status_child = status,
                            location_child = listtotal.Rows[i]["Location"].ToString(),
                            approved_by_child = user,
                            solution_child = listtotal.Rows[i]["Solution"].ToString() + message,

                            id1_child = listtotal.Rows[i]["path1ID"].ToString(),
                            doc1_child = doc_file_1,
                            id2_child = listtotal.Rows[i]["path2ID"].ToString(),
                            doc2_child = doc_file_2,

                            id = listtotal.Rows[i]["Id"].ToString(),

                            bitmap = imageSource,
                            color = status.color,
                            isEnable = isEnable
                        });
                    }
                }

                thong_tin_clash_child.ItemsSource = my_child;

                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_clash_child.ItemsSource);
                view.SortDescriptions.Add(new SortDescription("clash_child_sort", ListSortDirection.Ascending));

                view.Filter = Filter_ten_vat_lieu;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        private bool Filter_ten_vat_lieu(object item)
        {
            string data_search = search_child.Text;
            if (string.IsNullOrEmpty(data_search))
                return true;
            else
                return ((item as child).clash_child.IndexOf(data_search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (item as child).status_child.name.IndexOf(data_search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (item as child).location_child.IndexOf(data_search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (item as child).approved_by_child.IndexOf(data_search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (item as child).id1_child.IndexOf(data_search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (item as child).id2_child.IndexOf(data_search, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        //----------------------------------------------------------
        private void Search_Material_Project(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(search_child.Text))
            {
                CollectionViewSource.GetDefaultView(thong_tin_clash_child.ItemsSource).Refresh();
            }
        }

        //----------------------------------------------------------
        private void get_count_state()
        {

            try
            {
                if (thong_tin_clash_child.Items.Count > 0)
                {
                    List<count_status> list = new List<count_status>(my_child.GroupBy(x => new
                    {
                        x.status_child.name
                    }).Select(x => new count_status()
                    {
                        status = x.Key.name,
                        count = x.Count()
                    }));
                    int count = 0;
                    if (list.Any(x => x.status == Comments.Name) == false) Comments.Text = Comments.Name + "          " + count; 
                    else Comments.Text = Comments.Name + "          " + list.First(x => x.status == Comments.Name).count;
                    if (list.Any(x => x.status == Resolved.Name) == false) Resolved.Text = Resolved.Name + "          " + count;
                    else Resolved.Text = Resolved.Name + "          " + list.First(x => x.status == Resolved.Name).count;
                    if (list.Any(x => x.status == Approved.Name) == false) Approved.Text = Approved.Name + "          " + count;
                    else Approved.Text = Approved.Name + "          " + list.First(x => x.status == Approved.Name).count;
                    if (list.Any(x => x.status == Ignore.Name) == false) Ignore.Text = Ignore.Name + "          " + count;
                    else Ignore.Text = Ignore.Name + "          " + list.First(x => x.status == Ignore.Name).count;
                    if (list.Any(x => x.status == New.Name) == false) New.Text = New.Name + "          " + count;
                    else New.Text = New.Name + "          " + list.First(x => x.status == New.Name).count;

                    Comments.Foreground = Source.list_status_child[0].color;
                    Resolved.Foreground = Source.list_status_child[1].color;
                    Approved.Foreground = Source.list_status_child[2].color;
                    Ignore.Foreground = Source.list_status_child[3].color;
                    New.Foreground = Source.list_status_child[4].color;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //---------------------------------------------------------------------------------------------------
        #region Clash Child Item
        private void Hight_Lights(object sender, MouseButtonEventArgs e)
        {
            my_Image = new ObservableCollection<Image_Solution>();
            try
            {
                child item = (child)thong_tin_clash_child.SelectedItem;
                if (item != null)
                {
                    foreach (ImageSource imageSource in item.bitmap)
                    {
                        my_Image.Add(new Image_Solution()
                        {
                            bitmap = imageSource
                        });
                    }
                }
                hinh_anh_minh_hoa.ItemsSource = my_Image;

                my_focus.thong_tin_clash_child = thong_tin_clash_child;
                e_focus.Raise();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //----------------------------------------------------------
        private void Change_Status_Child(object sender, EventArgs e)
        {
            try
            {
                List<string> Check_Finish = new List<string>();
                foreach (child item in my_child)
                {
                    status_data data = (status_data)item.status_child;
                    item.color = data.color;
                    Check_Finish.Add(data.name);
                    thong_tin_clash_child.Items.Refresh();
                }

                if (Check_Finish.Distinct().ToList().Count() == 1 && Check_Finish[0] == Source.list_status_child[2].name)
                {
                    parent item_parent = (parent)thong_tin_clash_parent.SelectedItem;
                    item_parent.status_parent = Source.list_status_parent[1].name;
                    item_parent.color = Source.list_status_parent[1].color;
                    thong_tin_clash_parent.Items.Refresh();
                }
                else
                {
                    parent item_parent = (parent)thong_tin_clash_parent.SelectedItem;
                    item_parent.status_parent = Source.list_status_parent[0].name;
                    item_parent.color = Source.list_status_parent[0].color;
                    thong_tin_clash_parent.Items.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion
        #endregion

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Update_Clash(object sender, RoutedEventArgs e)
        {
            try
            {
                List<string> result = new List<string>();
                if (thong_tin_clash_parent.SelectedItem != null)
                {
                    foreach (child item in my_child)
                    {
                        if (item.status_child.name != Source.list_status_child[2].name && item.status_child.name != Source.list_status_child[3].name && item.status_child.name != Source.list_status_child[4].name)
                        {
                            List<string> Para = new List<string>() { "@DBProjectNumber", "@DBId", "@DBStatus" };
                            List<string> Para_Values = new List<string>() { doc.ProjectInformation.Number, item.id, item.status_child.name };
                            result.Add(SQL.SQLWrite(SQL.path_connect_SQL_FileStream, "dbo.sp_update_clash_status", Source.type_Procedure, Para, Para_Values));
                        }
                    }
                    if (result.Any(x => x == "F") == false) MessageBox.Show("Update success!", "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //--------------------------------------------------------------------------------------------------------------------------------------------------------------
        private void Refresh_View(object sender, RoutedEventArgs e)
        {

        }
    }
}
