# SAP Business One Add-on Starter (C#, UI API + DI API)

A clean, empty starter project for building an **SAP Business One add-on** in C#.
Clone it, rename the prefix, and put your own logic where the `TODO` comments are.

It covers the parts every add-on needs and nothing else:

| File | What it shows |
|---|---|
| `Program.cs` | Connect to the running SAP B1 client (UI API), get the DI API company from the same session, register events |
| `Menu.cs` | Add a menu under **Modules**, open a form from an embedded XML layout |
| `Events/ItemEvents.cs` | React to clicks and field changes on system forms, add your own button to the A/R Invoice form, cancel a save |
| `Events/FormDataEvents.cs` | Run code **after a document is saved** (e.g. after an A/R Invoice is added) |
| `Events/AppEvents.cs` | Shut down cleanly when SAP B1 closes or the company changes |
| `Data/DiApi.cs` | Query helper, SQL Server / SAP HANA check, add an A/R Invoice with the DI API, release COM objects |
| `Data/UserTables.cs` | Create user tables (UDT) and user fields (UDF) on start-up, including Yes/No valid values |
| `Forms/SampleForm.srf` | A form layout (XML) loaded with `LoadBatchActions` |
| `Forms/SampleForm.b1f` | The same idea as a B1 Studio form class |

## Requirements

- SAP Business One client (9.3 / 10.0) installed on the development machine, **x64** for current versions
- SAP Business One **SDK** installed (adds the `SAPBusinessOneSDK` reference: `SAPbouiCOM.Framework`, `SAPbouiCOM`, `SAPbobsCOM`)
- Visual Studio 2019 or 2022 with .NET Framework 4.7.2

## Run it

1. Open `SapB1AddonStarter.sln` in Visual Studio and select the **x64** platform.
2. Start the SAP Business One client and log in to a **test company**.
3. Press **F5**. The add-on connects to the running client with the SDK development connection string.
4. Go to **Modules > My Add-on > Sample Form**, or open an A/R Invoice and look for the **My Action** button.

## Make it yours

1. Search for `TODO` in the project.
2. Replace the `SAS_` prefix (menu IDs, form IDs, table and field names) with your own unique prefix. Two add-ons with the same IDs clash.
3. Rename the project, namespace and assembly (`SapB1AddonStarter`) and set your details in `Properties/AssemblyInfo.cs`.
4. Add your tables and fields in `Data/UserTables.cs`, your logic in the event classes, and your forms in `Forms/`.

## Deploy

Build in **Release | x64**, then create the add-on installer and register it in SAP Business One
(**Administration > Add-Ons > Add-On Administration**) with the *Add-On Registration Data Generator* and
*Add-On Installer* (B1DE) tools from the SDK. Test on a copy of the company database first.

## Tips that save hours

- Release every COM object (`Recordset`, `Documents`, metadata objects) with `Marshal.ReleaseComObject`.
- Write SQL with double-quoted names (`"DocEntry"`) so it runs on SQL Server and SAP HANA.
- Never build SQL from user input; convert to `int`/`DateTime` first or validate it.
- Item events fire for **every** form: always filter on `FormTypeEx` first.
- Run long work (HTTP calls) on a background thread so the SAP B1 UI does not freeze.
- Do not store passwords or API keys in the code or `App.config`.

## Related

- [SAP Business One table reference](https://kodenova.com/tools/sap-b1-table-reference/) (OINV, ORDR, OCRD and 180+ more)
- [SQL Server to SAP HANA converter](https://kodenova.com/tools/sql-to-hana-converter/)
- [SAP B1 Service Layer guide](https://kodenova.com/insights/sap-b1-service-layer-guide/)
- [SAP B1 MCP server](https://github.com/kodenova/sap-b1-mcp) (connect ChatGPT or Claude to SAP Business One)

## License

MIT. Use it in your own and your clients' projects. Built and maintained by [Kodenova](https://kodenova.com).
Not affiliated with or endorsed by SAP SE. SAP and SAP Business One are trademarks of SAP SE.
