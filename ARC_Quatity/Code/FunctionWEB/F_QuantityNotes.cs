using ARC_Quatity.Data;
using ARC_Quatity.Data.BindingWEB;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ARC_Quatity.Code.FunctionWEB
{
    class F_QuantityNotes
    {
        public static void Get_Data_Notes_Web(string project_number, string Class, ObservableCollection<data_table_note> my_table_note, ListView thong_tin_quantity_total_web)
        {
            try
            {
                List<string> para = new List<string>() { "@DBProjectNumber", "@DBKey" };
                List<string> para_value = new List<string>() { project_number, Source.desciplines.First(x => x.name == Class).key };
                var data = SQL.SQLRead(Source.path_WEB, "dbo.sp_ReadData_QuantityNote", Source.type_Procedure, para, para_value);
                ObservableCollection<data_table_note> my_table_note_support = new ObservableCollection<data_table_note>();
                for (var i = 0; i < data.Rows.Count; i++)
                {
                    if (data.Rows[i]["MaterialId"].ToString().Contains("."))
                    {
                        if (JsonConvert.DeserializeObject<data_status>(data.Rows[i]["Status"].ToString()).open == true)
                        {
                            my_table_note_support.Add(new data_table_note()
                            {
                                block = data.Rows[i]["MaterialBlock"].ToString(),
                                level = data.Rows[i]["MaterialLevel"].ToString(),
                                ma_cong_tac = data.Rows[i]["MaterialId"].ToString(),
                                notes = JsonConvert.DeserializeObject<data_note>(data.Rows[i]["Notes"].ToString()).message,
                                id = data.Rows[i]["Id"].ToString(),
                                edited = JsonConvert.DeserializeObject<data_status>(data.Rows[i]["Status"].ToString()).edited
                            });
                        }
                    }
                }
                my_table_note = new ObservableCollection<data_table_note>(my_table_note_support.GroupBy(x => new
                {
                    x.block,
                    x.level,
                    x.ma_cong_tac
                }).Select(x => new data_table_note()
                {
                    block = x.Key.block,
                    level = x.Key.level,
                    ma_cong_tac = x.Key.ma_cong_tac,
                    notes = string.Join("\n",x.Select(y => y.notes)),
                    ids = x.Select(y => y.id).ToList(),
                    editeds = x.Select(y => y.edited).ToList()
                }));
                thong_tin_quantity_total_web.ItemsSource = my_table_note;

                CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(thong_tin_quantity_total_web.ItemsSource);
                view.SortDescriptions.Add(new SortDescription("block", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("level", ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription("ma_cong_tac", ListSortDirection.Descending));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
