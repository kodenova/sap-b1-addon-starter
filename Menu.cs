using System;
using System.IO;
using System.Reflection;
using System.Xml;
using SAPbouiCOM;
using Application = SAPbouiCOM.Framework.Application;

namespace SapB1AddonStarter
{
    /// <summary>
    /// Creates the add-on menu and opens forms when a menu entry is clicked.
    /// </summary>
    internal class Menu
    {
        // Menu IDs must be unique across ALL add-ons. Use your own prefix.
        // TODO: rename the "SAS_" prefix to something unique for your add-on.
        private const string RootMenuId = "SAS_ROOT";
        private const string SampleFormMenuId = "SAS_SAMPLE";

        // "43520" is the standard SAP B1 "Modules" menu.
        private const string ModulesMenuId = "43520";

        public void AddMenuItems()
        {
            Menus menus = Application.SBO_Application.Menus;

            // Top-level popup under Modules.
            if (!menus.Exists(RootMenuId))
            {
                var p = (MenuCreationParams)Application.SBO_Application.CreateObject(BoCreatableObjectType.cot_MenuCreationParams);
                p.Type = BoMenuType.mt_POPUP;
                p.UniqueID = RootMenuId;
                p.String = "My Add-on";          // TODO: your add-on name
                p.Enabled = true;
                p.Position = -1;                 // -1 = last position
                menus.Item(ModulesMenuId).SubMenus.AddEx(p);
            }

            // Entry that opens the sample form.
            if (!menus.Exists(SampleFormMenuId))
            {
                var p = (MenuCreationParams)Application.SBO_Application.CreateObject(BoCreatableObjectType.cot_MenuCreationParams);
                p.Type = BoMenuType.mt_STRING;
                p.UniqueID = SampleFormMenuId;
                p.String = "Sample Form";        // TODO: your form title
                menus.Item(RootMenuId).SubMenus.AddEx(p);
            }

            // TODO: add more menu entries here (one per form or action).
        }

        public void OnMenuEvent(ref MenuEvent pVal, out bool bubbleEvent)
        {
            bubbleEvent = true;
            if (pVal.BeforeAction) return;

            try
            {
                switch (pVal.MenuUID)
                {
                    case SampleFormMenuId:
                        // Option A: XML form layout embedded in the project (Forms\SampleForm.srf).
                        LoadFormFromResource("SampleForm.srf");

                        // Option B: B1 Studio form class (Forms\SampleForm.b1f + .cs):
                        //   new Forms.SampleForm().Show();
                        break;

                    // TODO: handle your other menu IDs here.
                }
            }
            catch (Exception ex)
            {
                Application.SBO_Application.MessageBox(ex.Message);
            }
        }

        /// <summary>
        /// Loads a form layout (XML) that is embedded as a resource in this assembly.
        /// </summary>
        public static void LoadFormFromResource(string fileName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string resourceName = asm.GetName().Name + ".Forms." + fileName;

            using (Stream stream = asm.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                    throw new FileNotFoundException("Embedded form not found: " + resourceName);

                using (var reader = new StreamReader(stream))
                {
                    var xml = new XmlDocument();
                    xml.LoadXml(reader.ReadToEnd());
                    Application.SBO_Application.LoadBatchActions(xml.InnerXml);
                }
            }
        }
    }
}
