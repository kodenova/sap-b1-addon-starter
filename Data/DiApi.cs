using System;
using System.Runtime.InteropServices;
using SAPbobsCOM;

namespace SapB1AddonStarter.Data
{
    /// <summary>
    /// Small DI API helpers.
    /// Rules that save hours:
    ///  - Always release COM objects (Recordset, documents, metadata objects) with Marshal.ReleaseComObject.
    ///  - Quote names with double quotes ("DocEntry") so the same SQL works on SQL Server and SAP HANA.
    ///  - Never build SQL from user text. Validate or convert input (int, date) first.
    /// </summary>
    internal static class DiApi
    {
        /// <summary>Returns the first column of the first row, or an empty string.</summary>
        public static string GetValue(string sql)
        {
            Recordset rs = null;
            try
            {
                rs = (Recordset)Program.Company.GetBusinessObject(BoObjectTypes.BoRecordset);
                rs.DoQuery(sql);
                return rs.EoF ? string.Empty : Convert.ToString(rs.Fields.Item(0).Value);
            }
            finally
            {
                Release(rs);
            }
        }

        /// <summary>True when the database is SAP HANA (useful when SQL syntax differs).</summary>
        public static bool IsHana()
        {
            return Program.Company.DbServerType == BoDataServerTypes.dst_HANADB;
        }

        /// <summary>
        /// Example: add an A/R Invoice with one line through the DI API.
        /// TODO: replace the sample values with your own data.
        /// </summary>
        public static int AddSampleInvoice(string cardCode, string itemCode, double quantity, double price)
        {
            Documents invoice = null;
            try
            {
                invoice = (Documents)Program.Company.GetBusinessObject(BoObjectTypes.oInvoices);
                invoice.CardCode = cardCode;
                invoice.DocDate = DateTime.Today;
                invoice.DocDueDate = DateTime.Today.AddDays(30);

                invoice.Lines.ItemCode = itemCode;
                invoice.Lines.Quantity = quantity;
                invoice.Lines.UnitPrice = price;
                // invoice.Lines.Add();   // call before filling the next line

                // Write your own user field on the header (field must exist, see UserTables.cs):
                // invoice.UserFields.Fields.Item("U_SAS_Status").Value = "N";

                if (invoice.Add() != 0)
                    throw new Exception(LastError());

                return int.Parse(Program.Company.GetNewObjectKey());   // new DocEntry
            }
            finally
            {
                Release(invoice);
            }
        }

        public static string LastError()
        {
            Program.Company.GetLastError(out int code, out string message);
            return code + ": " + message;
        }

        public static void Release(object comObject)
        {
            if (comObject != null)
                Marshal.ReleaseComObject(comObject);
        }
    }
}
