using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TextBox = System.Windows.Controls.TextBox;

namespace Naviswork_ClashComment
{
    class SQL
    {

        public static string path_connect_SQL_FileStream = "Server=18.141.116.111,1433\\SQLEXPRESS;Database=FileStreamDataBase;User Id = FilestreamUser; Password = fileconnect@123";
        public static string path_connect_SQL_WEB = "Server=18.141.116.111,1433\\SQLEXPRESS;Database=WEBDataBase;User Id = WebUser; Password = webconnect456";
        public static string path_connect_SQL_Manage = "Server=18.141.116.111,1433\\SQLEXPRESS;Database=ManageDataBase;User Id = ManageUser; Password = manage @connect789";
        public static string path_connect_SQL_Quatity = "Server=18.141.116.111,1433\\SQLEXPRESS;Database=QuatityDataBase;User Id = RevitUser; Password = revit @connect";
        public static string path_connect_SQL_Allplan = "Server=18.141.116.111,1433\\SQLEXPRESS;Database=AllplanDataBase;User Id = AllplanUser; Password = allplan @connect";
        public static string path_connect_SQL_Revit = "Server=18.141.116.111,1433\\SQLEXPRESS;Database=RevitDataBase;User Id = RevitUser; Password = revit @connect";

        //Read-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static DataTable SQLRead(string connectString, string procedure, string commandType, List<string> parameters, List<string> parametersValue)
        {
            DataTable dtNew = new DataTable();
            try
            {
                SqlConnection connection = new System.Data.SqlClient.SqlConnection(connectString);
                SqlCommand commandDelete = new SqlCommand(procedure, connection);
                if (commandType == Source.type_Procedure)
                {
                    commandDelete.CommandType = CommandType.StoredProcedure;
                    if (parameters.Count > 0)
                    {
                        for (var i = 0; i < parameters.Count; i++)
                        {
                            commandDelete.Parameters.Add(new SqlParameter(parameters[i], parametersValue[i])).Value = parametersValue[i];
                        }
                    }
                }
                SqlDataAdapter adNew = new SqlDataAdapter(commandDelete);
                adNew.Fill(dtNew);

                connection.Open();
                commandDelete.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return dtNew;
        }

        //Write-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static string SQLWrite(string connectString, string procedure, string commandType, List<string> parameters, List<string> parametersValue)
        {
            try
            {
                SqlConnection connection = new System.Data.SqlClient.SqlConnection(connectString);
                SqlCommand commandDelete = new SqlCommand(procedure, connection);
                if (commandType == Source.type_Procedure)
                {
                    commandDelete.CommandType = CommandType.StoredProcedure;
                    if (parameters.Count > 0)
                    {
                        for (var i = 0; i < parameters.Count; i++)
                        {
                            commandDelete.Parameters.Add(new SqlParameter(parameters[i], parametersValue[i])).Value = parametersValue[i];
                        }
                    }
                }
                connection.Open();
                commandDelete.ExecuteNonQuery();
                connection.Close();
                return "S";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return "F";
            }
        }

        //Delete-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static string SQLDelete(string connectString, string procedure, string commandType, List<string> parameters, List<string> parametersValue)
        {
            try
            {
                SqlConnection connection = new System.Data.SqlClient.SqlConnection(connectString);
                SqlCommand commandDelete = new SqlCommand(procedure, connection);
                if (commandType == Source.type_Procedure)
                {
                    commandDelete.CommandType = CommandType.StoredProcedure;
                    if (parameters.Count > 0)
                    {
                        for (var i = 0; i < parameters.Count; i++)
                        {
                            commandDelete.Parameters.Add(new SqlParameter(parameters[i], parametersValue[i])).Value = parametersValue[i];
                        }
                    }
                }
                connection.Open();
                commandDelete.ExecuteNonQuery();
                connection.Close();
                return "S";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return "F";
            }
        }

    }
}
