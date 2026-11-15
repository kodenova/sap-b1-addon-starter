using System;
using SAPbouiCOM;

namespace SapB1AddonStarter.Events
{
    /// <summary>
    /// Application-level events (shutdown, company change, language change).
    /// </summary>
    internal static class AppEvents
    {
        public static void OnAppEvent(BoAppEventTypes eventType)
        {
            switch (eventType)
            {
                case BoAppEventTypes.aet_ShutDown:
                case BoAppEventTypes.aet_CompanyChanged:
                case BoAppEventTypes.aet_ServerTerminition:
                    // SAP B1 is closing or the user switched company: release COM objects and exit.
                    // TODO: save anything your add-on keeps in memory before exiting.
                    if (Program.Company != null && Program.Company.Connected)
                        Program.Company.Disconnect();
                    Environment.Exit(0);
                    break;

                case BoAppEventTypes.aet_LanguageChanged:
                    // TODO: reload menu texts if your add-on supports several languages.
                    break;
            }
        }
    }
}
