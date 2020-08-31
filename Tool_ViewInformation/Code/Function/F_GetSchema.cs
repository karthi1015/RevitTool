using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using System;
using MessageBox = System.Windows.MessageBox;

namespace Tool_ViewInformation.Code.Function
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
    }
}
