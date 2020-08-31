using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI_Ribbon.Data
{
    class Source
    {
        public static string fail_string = "Field is required!";
        public static string fail_empty = "User or password not null or empty!";
        public static string success = "Login success!";
        public static string fail_incorrect = "User name or password is incorrect!";

        public static string type_Procedure = "StoredProcedure";
        public static string type_Query = "Query";

        public static string pathUserPassword = @"C:\Users\" + Environment.UserName + "\\AppData\\Roaming\\Pentacons\\User";

        public static string path_WEB = "Server=18.141.116.111,1433\\SQLEXPRESS;Database=WEBDataBase;User Id=WebUser; Password = webconnect456";
        public static string path_Manage = "Server=18.141.116.111,1433\\SQLEXPRESS;Database=ManageDataBase;User Id=ManageUser; Password = manage@connect789";
    }
}
