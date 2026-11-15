using System;
using SapB1AddonStarter.Data;
using SapB1AddonStarter.Events;
using Application = SAPbouiCOM.Framework.Application;

namespace SapB1AddonStarter
{
    /// <summary>
    /// Entry point of the add-on.
    /// SAP Business One starts the add-on and passes a connection string as args[0].
    /// In Visual Studio (debug), no argument is passed and the SDK uses the
    /// development connection string, so the SAP B1 client must already be running.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// DI API company object (same session as the logged-in SAP B1 user).
        /// Use it for queries and to add/update documents and master data.
        /// </summary>
        public static SAPbobsCOM.Company Company;

        [STAThread]
        private static void Main(string[] args)
        {
            try
            {
                // 1. Connect to the running SAP Business One client (UI API).
                Application app = args.Length < 1 ? new Application() : new Application(args[0]);

                // 2. Get the DI API company from the same session (single sign-on, no password needed).
                Company = (SAPbobsCOM.Company)Application.SBO_Application.Company.GetDICompany();

                // 3. Create your user tables (UDT) and user fields (UDF) once.
                //    TODO: add your own tables and fields in UserTables.EnsureMetadata().
                UserTables.EnsureMetadata();

                // 4. Add the add-on menu (Modules > My Add-on > Sample Form).
                var menu = new Menu();
                menu.AddMenuItems();
                app.RegisterMenuEventHandler(menu.OnMenuEvent);

                // 5. Subscribe to SAP B1 events.
                Application.SBO_Application.AppEvent += AppEvents.OnAppEvent;
                Application.SBO_Application.ItemEvent += ItemEvents.OnItemEvent;
                Application.SBO_Application.FormDataEvent += FormDataEvents.OnFormDataEvent;

                Application.SBO_Application.StatusBar.SetText(
                    "SAP B1 Add-on Starter connected.",
                    SAPbouiCOM.BoMessageTime.bmt_Short,
                    SAPbouiCOM.BoStatusBarMessageType.smt_Success);

                // 6. Keep the add-on running until SAP B1 closes it.
                app.Run();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("Add-on failed to start: " + ex.Message);
            }
        }
    }
}
