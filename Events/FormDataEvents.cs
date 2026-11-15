using System;
using System.Xml;
using SAPbouiCOM;
using SapB1AddonStarter.Data;
using Application = SAPbouiCOM.Framework.Application;

namespace SapB1AddonStarter.Events
{
    /// <summary>
    /// Form data events: fire when a document or master record is added, updated, loaded or deleted.
    /// This is the right place for "after the invoice is saved, do X" logic.
    /// BusinessObjectInfo.Type is the object type: "13" A/R Invoice, "14" A/R Credit Memo,
    /// "17" Sales Order, "15" Delivery, "18" A/P Invoice, "2" Business Partner, "4" Item, "203" A/R Down Payment.
    /// </summary>
    internal static class FormDataEvents
    {
        public static void OnFormDataEvent(ref BusinessObjectInfo info, out bool bubbleEvent)
        {
            bubbleEvent = true;

            try
            {
                // Example: run code after an A/R Invoice was added successfully.
                if (info.Type == "13"
                    && info.EventType == BoEventTypes.et_FORM_DATA_ADD
                    && !info.BeforeAction
                    && info.ActionSuccess)
                {
                    int docEntry = GetDocEntry(info.ObjectKey);

                    // Read data with the DI API (parameterless example, safe because docEntry is an int).
                    string docNum = DiApi.GetValue($@"SELECT ""DocNum"" FROM ""OINV"" WHERE ""DocEntry"" = {docEntry}");

                    // TODO: your logic here, for example:
                    //  - send the document to an external API (e-invoicing, e-commerce, WMS...)
                    //  - write the result back to a user field (UDF) on the document
                    //  - log the result in your own user table (UDT)
                    // Tip: run long calls (HTTP) on a background thread so the SAP B1 UI does not freeze.

                    Application.SBO_Application.StatusBar.SetText(
                        "Invoice " + docNum + " saved. Add-on logic can run here.",
                        BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                }

                // TODO: handle other object types and event types (et_FORM_DATA_UPDATE, et_FORM_DATA_LOAD...).
            }
            catch (Exception ex)
            {
                Application.SBO_Application.StatusBar.SetText(
                    "FormDataEvent error: " + ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }

        /// <summary>
        /// ObjectKey is XML, for example: &lt;DocumentParams&gt;&lt;DocEntry&gt;125&lt;/DocEntry&gt;&lt;/DocumentParams&gt;
        /// </summary>
        private static int GetDocEntry(string objectKey)
        {
            var xml = new XmlDocument();
            xml.LoadXml(objectKey);
            return int.Parse(xml.InnerText);
        }
    }
}
