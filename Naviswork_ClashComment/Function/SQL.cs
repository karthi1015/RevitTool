﻿using Autodesk.Revit.UI;
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
