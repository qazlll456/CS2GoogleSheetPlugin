using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;

namespace CS2GoogleSheetPlugin
{
    public class GoogleSheetPluginConfig : BasePluginConfig, IPluginConfig<GoogleSheetPluginConfig>
    {
        [JsonIgnore]
        public GoogleSheetPluginConfig Config { get; set; }

        public GoogleSheetSettings GoogleSheetSettings { get; set; } = new GoogleSheetSettings();
        public CredentialsSettings CredentialsSettings { get; set; } = new CredentialsSettings();
        public PluginSettings PluginSettings { get; set; } = new PluginSettings();

        public GoogleSheetPluginConfig()
        {
            Config = this;
        }

        public void OnConfigParsed(GoogleSheetPluginConfig config)
        {
            Config = config;
        }
    }

    public class GoogleSheetSettings
    {
        [JsonIgnore]
        public string SpreadsheetIdInfo1 { get; set; } = "Create a Google Sheet and Get the ID: Go to https://sheets.google.com, sign in, and click '+ New' to create a spreadsheet. In the URL (e.g., https://docs.google.com/spreadsheets/d/SPREADSHEET_ID/edit#gid=0), copy the SPREADSHEET_ID (e.g., example-spreadsheet-id-123456789) and paste it into the SpreadsheetId field.";

        [JsonIgnore]
        public string SpreadsheetIdInfo2 { get; set; } = "Enable the Google Sheets API: Go to https://console.cloud.google.com, create/select a project, go to 'APIs & Services' > 'Library', search for 'Google Sheets API', and click 'Enable'. Enable billing if prompted (free tier is usually sufficient).";

        [JsonIgnore]
        public string SpreadsheetIdInfo3 { get; set; } = "Create a Service Account and Key: In Google Cloud Console, go to 'IAM & Admin' > 'Service Accounts', click 'Create Service Account', name it (e.g., 'CS2GoogleSheetPlugin'), and click 'Create and Continue'. Skip optional steps, click 'Done'. In the service account, go to 'Keys', click 'Add Key' > 'Create new key', select 'JSON', and download the key (e.g., example-project-123456.json).";

        [JsonIgnore]
        public string SpreadsheetIdInfo4 { get; set; } = "Share the Sheet with the Service Account: Open the JSON file, find the client_email (e.g., example-service-account@example-project.iam.gserviceaccount.com). In your Google Sheet, click 'Share', paste the client_email, set permission to 'Editor', and click 'Send' or 'Done'. Copy the JSON file to JsonFilePath and update JsonFileName in this config.";

        [JsonIgnore]
        public string SpreadsheetIdInfo5 { get; set; } = "Test the Plugin: Update all config fields (SpreadsheetId, JsonFilePath, JsonFileName), restart the CS2 server, and test with css_gs_connect to confirm connection, then css_gsget and css_gsset 'test'. Common issues: 1) 'No permission' - ensure client_email is shared as Editor. 2) 'API key not found' - verify API is enabled and JSON file is correct. 3) 'Network error' - check server internet. 4) Reload fails - restart server due to CounterStrikeSharp bug.";

        public string SpreadsheetId { get; set; } = "example-spreadsheet-id-123456789";

        [JsonIgnore]
        public string SheetNameInfo { get; set; } = "The name of the sheet in the Google Sheet to interact with (e.g., 'ExampleSheet'). This is the tab name visible at the bottom of the Google Sheet.";

        public string SheetName { get; set; } = "ExampleSheet";

        [JsonIgnore]
        public string CellNameInfo { get; set; } = "The cell in the sheet to interact with (e.g., 'B2' for cell B2). Combine with SheetName to form the range (e.g., 'ExampleSheet!B2').";

        public string CellName { get; set; } = "B2";
    }

    public class CredentialsSettings
    {
        [JsonIgnore]
        public string JsonFilePathInfo { get; set; } = "The directory path where the Google Sheets API credentials JSON file is stored. Ensure the path ends with a slash (/).";

        public string JsonFilePath { get; set; } = "/path/to/your/server/addons/counterstrikesharp/plugins/CS2GoogleSheetPlugin/";

        [JsonIgnore]
        public string JsonFileNameInfo { get; set; } = "The name of the Google Sheets API credentials JSON file (e.g., 'example-project-123456.json'). This file should be placed in the JsonFilePath directory.";

        public string JsonFileName { get; set; } = "example-project-123456.json";
    }

    public class PluginSettings
    {
        [JsonIgnore]
        public string CacheDurationMinutesInfo { get; set; } = "The duration (in minutes) to cache the data retrieved from Google Sheets to reduce API calls. Set to 0 to disable caching.";

        public int CacheDurationMinutes { get; set; } = 2;

        [JsonIgnore]
        public string DebugLogInfo { get; set; } = "Enables or disables debug logging. When true, detailed logs (e.g., 'Executing command', 'Fetching data') are printed to the console. Set to false to reduce log output.";

        public bool DebugLog { get; set; } = true;
    }
}