using SAPbouiCOM.Framework;

namespace SapB1AddonStarter.Forms
{
    /// <summary>
    /// B1 Studio form class. Open SampleForm.b1f with the SAP B1 Studio designer
    /// (installed with the SDK) to add items; the designer updates OnInitializeComponent.
    /// Open it from code with: new SampleForm().Show();
    /// </summary>
    [FormAttribute("SapB1AddonStarter.Forms.SampleForm", "Forms/SampleForm.b1f")]
    internal class SampleForm : UserFormBase
    {
        public SampleForm()
        {
        }

        /// <summary>Called by the framework after the form is created. Get your items here.</summary>
        public override void OnInitializeComponent()
        {
            // TODO: get items, for example:
            // this.txtName = (SAPbouiCOM.EditText)this.GetItem("txtName").Specific;
        }

        /// <summary>Called by the framework before the form is created. Wire item events here.</summary>
        public override void OnInitializeFormEvents()
        {
            // TODO: subscribe to form events, for example:
            // this.LoadAfter += (ref SAPbouiCOM.SBOItemEventArg pVal) => { };
        }
    }
}
