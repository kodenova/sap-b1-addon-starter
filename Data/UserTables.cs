using System;
using SAPbobsCOM;

namespace SapB1AddonStarter.Data
{
    /// <summary>
    /// Creates user-defined tables (UDT) and user-defined fields (UDF) the add-on needs.
    /// Runs at every start; each method skips objects that already exist.
    /// Notes:
    ///  - Metadata changes need a user with superuser rights and no other user in the add-on's tables.
    ///  - Table names here have NO "@" (SAP adds it: "SAS_LOG" becomes "@SAS_LOG").
    ///  - Field names have NO "U_" (SAP adds it: "Status" becomes "U_Status").
    /// </summary>
    internal static class UserTables
    {
        public static void EnsureMetadata()
        {
            // TODO: replace these examples with the tables and fields your add-on needs.

            // A user table for logs.
            CreateTable("SAS_LOG", "Add-on Starter Log", BoUTBTableType.bott_NoObject);
            CreateField("@SAS_LOG", "DocEntry", "Document Entry", BoFieldTypes.db_Numeric, 11);
            CreateField("@SAS_LOG", "Message", "Message", BoFieldTypes.db_Alpha, 254);

            // A user field on the A/R Invoice header (OINV) with Yes/No values.
            CreateField("OINV", "SAS_Status", "Add-on Status", BoFieldTypes.db_Alpha, 1,
                validValues: new[,] { { "N", "No" }, { "Y", "Yes" } }, defaultValue: "N");
        }

        public static bool TableExists(string tableName)
        {
            UserTablesMD md = null;
            try
            {
                md = (UserTablesMD)Program.Company.GetBusinessObject(BoObjectTypes.oUserTables);
                return md.GetByKey(tableName);
            }
            finally
            {
                DiApi.Release(md);
            }
        }

        public static bool FieldExists(string tableName, string fieldName)
        {
            // CUFD stores user fields. TableID keeps the "@" for user tables.
            string sql = $@"SELECT COUNT(*) FROM ""CUFD"" WHERE ""TableID"" = '{tableName.Replace("'", "''")}' AND ""AliasID"" = '{fieldName.Replace("'", "''")}'";
            return DiApi.GetValue(sql) != "0";
        }

        public static void CreateTable(string tableName, string description, BoUTBTableType type)
        {
            if (TableExists(tableName)) return;

            UserTablesMD md = null;
            try
            {
                md = (UserTablesMD)Program.Company.GetBusinessObject(BoObjectTypes.oUserTables);
                md.TableName = tableName;
                md.TableDescription = description;
                md.TableType = type;
                if (md.Add() != 0)
                    throw new Exception("Create table " + tableName + " failed: " + DiApi.LastError());
            }
            finally
            {
                DiApi.Release(md);
                GC.Collect();   // metadata objects must be fully released before the next one
            }
        }

        public static void CreateField(string tableName, string fieldName, string description,
            BoFieldTypes type, int size = 0,
            BoFldSubTypes subType = BoFldSubTypes.st_None,
            string[,] validValues = null, string defaultValue = null, string linkedTable = null)
        {
            if (FieldExists(tableName, fieldName)) return;

            UserFieldsMD md = null;
            try
            {
                md = (UserFieldsMD)Program.Company.GetBusinessObject(BoObjectTypes.oUserFields);
                md.TableName = tableName;
                md.Name = fieldName;
                md.Description = description;
                md.Type = type;
                if (subType != BoFldSubTypes.st_None) md.SubType = subType;

                if (size > 0)
                {
                    if (type == BoFieldTypes.db_Numeric) md.EditSize = size;
                    else if (type == BoFieldTypes.db_Alpha) md.Size = size;
                }

                if (validValues != null)
                {
                    for (int i = 0; i < validValues.GetLength(0); i++)
                    {
                        md.ValidValues.Value = validValues[i, 0];
                        md.ValidValues.Description = validValues[i, 1];
                        md.ValidValues.Add();
                    }
                }

                if (!string.IsNullOrEmpty(defaultValue)) md.DefaultValue = defaultValue;
                if (!string.IsNullOrEmpty(linkedTable)) md.LinkedTable = linkedTable;

                if (md.Add() != 0)
                    throw new Exception("Create field " + tableName + "." + fieldName + " failed: " + DiApi.LastError());
            }
            finally
            {
                DiApi.Release(md);
                GC.Collect();
            }
        }

        // TODO: register a UDO (user-defined object) here if your add-on needs its own document type
        // with Find/Add/Update in the SAP B1 UI (SAPbobsCOM.UserObjectsMD).
    }
}
