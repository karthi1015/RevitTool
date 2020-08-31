using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.ExtensibleStorage;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MessageBox = System.Windows.MessageBox;
using Excel = Microsoft.Office.Interop.Excel;
using ARC_Quatity.Data.Binding;
using ARC_Quatity.Data;

namespace ARC_Quatity.Code.Function
{
    class F_Upload
    {
        //----------------------------------------------------------
        public static string Upload(Autodesk.Revit.DB.Document doc, List<List<List<string>>> values, List<string> material_id, string id_file)
        {
            string result = "F";
            try
            {
                int column_material = 10;
                int column_element = 11;
                List<List<string>> value_material = new List<List<string>>();

                DataTable dt = new DataTable();
                for (int i = 0; i < column_material; i++)
                {
                    dt.Columns.Add(new DataColumn(i.ToString(), typeof(string)));
                }
                foreach (string key in material_id)
                {
                    List<string> support = new List<string>();
                    for (int i = 0; i < column_material; i++)
                    {
                        if (i == 0) support.Add(id_file);
                        else if (i == 4) support.Add(key);
                        else support.Add("-1");
                    }
                    value_material.Add(support);
                }
                foreach (List<string> value in value_material)
                {
                    dt.Rows.Add(dt.NewRow().ItemArray = value.ToArray());
                }

                DataTable dt1 = new DataTable();
                for (int i = 0; i < column_material; i++)
                {
                    dt1.Columns.Add(new DataColumn(i.ToString(), typeof(string)));
                }
                foreach (List<string> value in values[0])
                {
                    dt1.Rows.Add(dt1.NewRow().ItemArray = value.ToArray());
                }

                DataTable dt2 = new DataTable();
                for (int i = 0; i < column_element; i++)
                {
                    dt2.Columns.Add(new DataColumn(i.ToString(), typeof(string)));
                }
                foreach (List<string> value in values[1])
                {
                    dt2.Rows.Add(dt2.NewRow().ItemArray = value.ToArray());
                }

                List<string> Para1 = new List<string>() { "@DBProjectNumber", "@DBIdFile", "@DBTable_All", "@DBTable_Upload", "@DBTable_Upload_1" };
                List<object> Para1_Values = new List<object>() { doc.ProjectInformation.Number, id_file, dt, dt1, dt2 };
                var result_sql_delete = SQL.SQLDelete(Source.path_revit, "dbo.sp_delete_quantity_support", Source.type_Procedure, Para1, Para1_Values);

                List<string> Para2 = new List<string>() { "@DBProjectNumber", "@DBTable_Upload", "@DBTable_Upload_1" };
                List<object> Para2_Values = new List<object>() { doc.ProjectInformation.Number, dt1, dt2 };
                var result_sql_upload = SQL.SQLWrite(Source.path_revit, "dbo.sp_insert_quantity", Source.type_Procedure, Para2, Para2_Values);

                if (result_sql_delete == "S" && result_sql_upload == "S")
                {
                    result = "S";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return result;
        }
    }
}
