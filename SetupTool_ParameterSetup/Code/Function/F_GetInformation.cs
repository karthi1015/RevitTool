using Autodesk.Revit.DB;
using Newtonsoft.Json;
using SetupTool_ParameterSetup.Data.Binding;
using SetupTool_ParameterSetup.Data.BindingWEB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SetupTool_ParameterSetup.Code.Function
{
    class F_GetInformation
    {
        //----------------------------------------------------------
        public static void get_information(Document doc, ComboBox number)
        {
            try
            {
                List<data_information> information = new List<data_information>();
                information.Add(new data_information()
                {
                    project_name = "xx",
                    project_address = "xx",
                    project_number = "xx"
                });

                var listtotal = SQL.SQLRead(Source.path_WEB, "select BucketKey,BucketData from dbo.ProjectInformation", Source.type_Query, new List<string>(), new List<string>());
                for (var i = 0; i < listtotal.Rows.Count; i++)
                {
                    data_information_WEB data = JsonConvert.DeserializeObject<data_information_WEB>(listtotal.Rows[i]["BucketData"].ToString());
                    information.Add(new data_information()
                    {
                        project_number = listtotal.Rows[i]["BucketKey"].ToString(),
                        project_address = data.bucketLocation,
                        project_name = data.bucketName
                    });
                }
                number.ItemsSource = information;
                number.SelectedItem = information.First(x => x.project_number == (doc.ProjectInformation.Number == "0001" ? "xx" : doc.ProjectInformation.Number));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
