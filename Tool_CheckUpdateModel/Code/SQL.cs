using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tool_CheckUpdateModel.Function
{
    class SQL
    {
        static string type = "StoredProcedure";

        //Read-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static DataTable SQLRead(string connectString, string procedure, string commandType, List<string> Parameters, List<string> ParametersValue)
        {
            DataTable dtNew = new DataTable();
            try
            {
                SqlConnection connection = new System.Data.SqlClient.SqlConnection(connectString);
                SqlCommand commandDelete = new SqlCommand(procedure, connection);
                if (commandType == type)
                {
                    commandDelete.CommandType = CommandType.StoredProcedure;
                    if (Parameters.Count > 0)
                    {
                        for (var i = 0; i < Parameters.Count; i++)
                        {
                            commandDelete.Parameters.Add(new SqlParameter(Parameters[i], ParametersValue[i])).Value = ParametersValue[i];
                        }
                    }
                }
                SqlDataAdapter adNew = new SqlDataAdapter(commandDelete);
                adNew.Fill(dtNew);

                connection.Open();
                commandDelete.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception)
            {

            }
            return dtNew;
        }

        //Write-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static string SQLWrite(string connectString, string procedure, string commandType, List<string> Parameters, List<string> ParametersValue)
        {
            try
            {
                SqlConnection connection = new System.Data.SqlClient.SqlConnection(connectString);
                SqlCommand commandDelete = new SqlCommand(procedure, connection);
                if (commandType == type)
                {
                    commandDelete.CommandType = CommandType.StoredProcedure;
                    if (Parameters.Count > 0)
                    {
                        for (var i = 0; i < Parameters.Count; i++)
                        {
                            commandDelete.Parameters.Add(new SqlParameter(Parameters[i], ParametersValue[i])).Value = ParametersValue[i];
                        }
                    }
                }
                connection.Open();
                commandDelete.ExecuteNonQuery();
                connection.Close();
                return "S";
            }
            catch (Exception)
            {
                return "F";
            }
        }

        //Delete-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public static string SQLDelete(string connectString, string procedure, string commandType, List<string> Parameters, List<string> ParametersValue)
        {
            try
            {
                SqlConnection connection = new System.Data.SqlClient.SqlConnection(connectString);
                SqlCommand commandDelete = new SqlCommand(procedure, connection);
                if (commandType == type)
                {
                    commandDelete.CommandType = CommandType.StoredProcedure;
                    if (Parameters.Count > 0)
                    {
                        for (var i = 0; i < Parameters.Count; i++)
                        {
                            commandDelete.Parameters.Add(new SqlParameter(Parameters[i], ParametersValue[i])).Value = ParametersValue[i];
                        }
                    }
                }
                connection.Open();
                commandDelete.ExecuteNonQuery();
                connection.Close();
                return "S";
            }
            catch (Exception)
            {
                return "F";
            }
        }
    }
}
