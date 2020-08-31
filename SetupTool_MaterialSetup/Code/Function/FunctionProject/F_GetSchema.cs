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
using SetupTool_MaterialSetup.Data.Binding;

namespace SetupTool_MaterialSetup.Code.Function
{
    class F_GetSchema
    {
        //----------------------------------------------------------
        public static string Check_Para_And_Get_Para(Material material, string guid, string name_para)
        {
            string value = "";
            try
            {
                Schema getSchema = Schema.Lookup(new Guid(guid));
                if (getSchema != null)
                {
                    Entity ent = material.GetEntity(getSchema);
                    if (ent.Schema != null)
                    {
                        value = ent.Get<string>(getSchema.GetField(name_para));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return value;
        }

        //----------------------------------------------------------
        public static void SetDataStorage(Material material, string guid, string name_para, string value)
        {
            try
            {
                SchemaBuilder sb1 = new SchemaBuilder(new Guid(guid));
                sb1.SetReadAccessLevel(AccessLevel.Public);
                sb1.SetWriteAccessLevel(AccessLevel.Public);
                sb1.SetVendorId("pentacons");
                sb1.SetSchemaName(name_para);

                FieldBuilder fieldUser = sb1.AddSimpleField(name_para, typeof(string));
                fieldUser.SetDocumentation("Set");
                Schema schema = sb1.Finish();
                Entity entity = new Entity(schema);

                Field field = schema.GetField(name_para);
                entity.Set(field, value);
                material.SetEntity(entity);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
