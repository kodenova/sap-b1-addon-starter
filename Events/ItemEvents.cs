using System;
using SAPbouiCOM;
using Application = SAPbouiCOM.Framework.Application;

namespace SapB1AddonStarter.Events
{
    /// <summary>
    /// Item events: clicks, field changes, form load, etc. on ANY SAP B1 form.
    /// Filter by FormTypeEx first, because this handler fires for every form in the client.
    /// Common system form types: "133" A/R Invoice, "139" Sales Order, "149" Sales Quotation,
    /// "140" Delivery, "179" A/R Credit Memo, "141" A/P Invoice, "134" Business Partner, "150" Item Master.
    /// </summary>
    internal static class ItemEvents
    {
        private const string SampleButtonId = "SAS_BTN";

        public static void OnItemEvent(string formUID, ref ItemEvent pVal, out bool bubbleEvent)
        {
            bubbleEvent = true;

            try
            {
                // Example 1: add your own button to the A/R Invoice form when it loads.
                if (pVal.FormTypeEx == "133" && pVal.EventType == BoEventTypes.et_FORM_LOAD && !pVal.BeforeAction)
                {
                    AddButtonToForm(Application.SBO_Application.Forms.Item(formUID));
                }

                // Example 2: react when your button is clicked.
                if (pVal.FormTypeEx == "133" && pVal.ItemUID == SampleButtonId
                    && pVal.EventType == BoEventTypes.et_ITEM_PRESSED && !pVal.BeforeAction)
                {
                    // TODO: put your button logic here (call an API, open a form, update a UDF...).
                    Application.SBO_Application.StatusBar.SetText(
                        "Sample button clicked.", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                }

                // Example 3: validate before SAP B1 saves (BeforeAction = true lets you cancel).
                // if (pVal.FormTypeEx == "133" && pVal.ItemUID == "1" && pVal.BeforeAction
                //     && pVal.EventType == BoEventTypes.et_ITEM_PRESSED)
                // {
                //     // TODO: your validation. To stop the save:
                //     // Application.SBO_Application.StatusBar.SetText("Reason...", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                //     // bubbleEvent = false;
                // }
            }
            catch (Exception ex)
            {
                Application.SBO_Application.StatusBar.SetText(
                    "ItemEvent error: " + ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }

        /// <summary>
        /// Adds a button next to the standard "Cancel" button (item "2") of a system form.
        /// </summary>
        private static void AddButtonToForm(Form form)
        {
            try
            {
                form.Items.Item(SampleButtonId);   // already exists
                return;
            }
            catch
            {
                // Not found: create it below.
            }

            Item cancel = form.Items.Item("2");
            Item button = form.Items.Add(SampleButtonId, BoFormItemTypes.it_BUTTON);
            button.Left = cancel.Left + cancel.Width + 5;
            button.Top = cancel.Top;
            button.Width = 100;
            button.Height = cancel.Height;
            ((Button)button.Specific).Caption = "My Action";   // TODO: your button text
        }
    }
}
