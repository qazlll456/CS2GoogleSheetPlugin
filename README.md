# CS2GoogleSheetPlugin

## Overview
- **Name**: CS2GoogleSheetPlugin
- **Version**: 1.0.0
- **Author**: qazlll456 from HK with xAI assistance
- **Description**: A CounterStrikeSharp plugin for CS2 that allows interaction with Google Sheets, enabling server administrators to get, set, append, and remove data from a specified cell in a Google Sheet. This plugin demonstrates the potential of integrating Google Sheets with CS2 servers, allowing for dynamic data management, such as storing player stats, server logs, or other game-related information in a cloud-based spreadsheet.

## Support
If you enjoy this plugin, consider supporting my work!  
Money, Steam games, or any valuable contribution is welcome.  
[Donate - Streamlabs, PayPal](https://streamlabs.com/BKCqazlll456/tip)

## Features
- **Get Data**: Retrieve data from a specified cell (`css_gsget`).
- **Set Data**: Write data to a specified cell (`css_gsset "value"`).
- **Add Data**: Add data to the existing cell content (`css_gsadd "value"`).
- **Remove Data**: Remove specific data from the cell (`css_gsremove "value"`).
- **Check Connection**: Verify connection to Google Sheets API (`css_gs_connect`).

## Purpose
This plugin serves as a proof of concept to showcase the potential of using Google Sheets with CS2 servers. By integrating with Google Sheets, server administrators can:
- Store and manage game data (e.g., player scores, match results) in a cloud-based spreadsheet.
- Create dynamic leaderboards or logs that can be accessed and edited outside the game.
- Automate data workflows by connecting CS2 server events to Google Sheets for real-time updates.
This plugin provides a foundation for more advanced integrations, such as syncing player data across servers or generating reports.

## Setting Up Google Sheets
Follow these steps to set up Google Sheets for use with the plugin:

### Step 1: Create a Google Sheet
1. Go to [sheets.google.com](https://sheets.google.com), sign in, and click **+ New** to create a spreadsheet.
2. In the URL (e.g., `https://docs.google.com/spreadsheets/d/SPREADSHEET_ID/edit#gid=0`), copy the `SPREADSHEET_ID` (e.g., `example-spreadsheet-id-123456789`).
3. Update the `SpreadsheetId` field in `CS2GoogleSheetPlugin.json` with this ID.

### Step 2: Enable the Google Sheets API
1. Go to [console.cloud.google.com](https://console.cloud.google.com) and create or select a project.
2. Navigate to **APIs & Services** > **Library**, search for **Google Sheets API**, and click **Enable**.
3. Enable billing if prompted (the free tier is usually sufficient).

### Step 3: Create a Service Account and Key
1. In Google Cloud Console, go to **IAM & Admin** > **Service Accounts**.
2. Click **Create Service Account**, name it (e.g., `CS2GoogleSheetPlugin`), and click **Create and Continue**.
3. Skip optional steps and click **Done**.
4. In the service account, go to **Keys**, click **Add Key** > **Create new key**, select **JSON**, and download the key (e.g., `example-project-123456.json`).

### Step 4: Share the Google Sheet with the Service Account
1. Open the downloaded JSON file and find the `client_email` (e.g., `example-service-account@example-project.iam.gserviceaccount.com`).
2. In your Google Sheet, click **Share**, paste the `client_email`, set the permission to **Editor**, and click **Send** or **Done**.

### Step 5: Test the Plugin
1. Restart your CS2 server.
2. Use the `css_gs_connect` command to confirm the connection to Google Sheets.
3. Test with `css_gsget` to retrieve data and `css_gsset "test"` to set data.
4. Check the server logs for errors. Common issues:
   - **"No permission"**: Ensure the `client_email` is shared as an Editor in the Google Sheet.
   - **"API key not found"**: Verify the Google Sheets API is enabled and the JSON file is correct.
   - **"Network error"**: Check the server’s internet connection.
   - **Reload fails**: Restart the server due to a CounterStrikeSharp bug.

## Installation

### Prerequisites
Before installing the plugin, ensure you have the following:

- **CounterStrike 2 (CS2) Server**: A running CS2 server where the plugin will be installed.
- **CounterStrikeSharp**: The plugin framework for CS2. Install it by following the official guide at [CounterStrikeSharp Docs](https://docs.cssharp.dev/). Ensure it’s set up in your server’s `csgo/addons/counterstrikesharp/` directory.
- **.NET 8.0 SDK**: Required to build the plugin. Download and install it from [dotnet.microsoft.com](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).
- **Google Cloud Project with Sheets API Enabled**:
  - Create a project in the [Google Cloud Console](https://console.cloud.google.com).
  - Enable the Google Sheets API: Go to **APIs & Services** > **Library**, search for **Google Sheets API**, and click **Enable**.
  - Enable billing if prompted (the free tier is usually sufficient).
- **Google Sheet**: A Google Sheet to interact with. Create one at [sheets.google.com](https://sheets.google.com).
- **Service Account Credentials**: A JSON key file for a Google Cloud service account with access to the Google Sheets API. See the "Setting Up Google Sheets" section for details.

### Steps
1. **Obtain the Plugin DLL**:
   - **Option 1: Build from Source**:
     - Clone or download the repository from GitHub.
     - Navigate to the project directory (`CS2GoogleSheetPlugin`) and run:
       ```bash
       dotnet build
       ```
     - The compiled `CS2GoogleSheetPlugin.dll` will be in the `bin/Debug/net8.0/` directory.
   - **Option 2: Download Precompiled DLL**:
     - Download the latest `CS2GoogleSheetPlugin.dll` from the [Releases](https://github.com/qazlll456/CS2GoogleSheetPlugin/releases) page if you don’t want to build from source.
2. **Copy the Plugin**:
   - Copy the `CS2GoogleSheetPlugin.dll` to your CS2 server’s plugin directory: `csgo/addons/counterstrikesharp/plugins/CS2GoogleSheetPlugin/`.
3. **Copy the Service Account JSON**:
   - Copy your Google Sheets API credentials JSON file (e.g., `example-project-123456.json`) to the plugin directory: `csgo/addons/counterstrikesharp/plugins/CS2GoogleSheetPlugin/`.
4. **Configure the Plugin**:
   - After the first run, the plugin will generate a `CS2GoogleSheetPlugin.json` file in `csgo/addons/counterstrikesharp/configs/plugins/CS2GoogleSheetPlugin/`.
   - Update this file with your Google Sheet details and credentials path (see the "Configuration" section below).
5. **Restart the Server**:
   - Restart your CS2 server to load the plugin.
6. **Test the Plugin**:
   - Use the `css_gs_connect` command to confirm the connection to Google Sheets.
   - Test with `css_gsget` and `css_gsset "test"` to verify functionality.

## Configuration
The plugin generates a `CS2GoogleSheetPlugin.json` file in `csgo/addons/counterstrikesharp/configs/plugins/CS2GoogleSheetPlugin/` with the following structure:

### Example with Instructions
This example includes detailed instructions for setting up each field:

```json
{
  "GoogleSheetSettings": {
    // Create a Google Sheet and Get the ID: Go to https://sheets.google.com, sign in, and click '+ New' to create a spreadsheet. In the URL (e.g., https://docs.google.com/spreadsheets/d/SPREADSHEET_ID/edit#gid=0), copy the SPREADSHEET_ID (e.g., example-spreadsheet-id-123456789) and paste it into the SpreadsheetId field.
    // Enable the Google Sheets API: Go to https://console.cloud.google.com, create/select a project, go to 'APIs & Services' > 'Library', search for 'Google Sheets API', and click 'Enable'. Enable billing if prompted (free tier is usually sufficient).
    // Create a Service Account and Key: In Google Cloud Console, go to 'IAM & Admin' > 'Service Accounts', click 'Create Service Account', name it (e.g., 'CS2GoogleSheetPlugin'), and click 'Create and Continue'. Skip optional steps, click 'Done'. In the service account, go to 'Keys', click 'Add Key' > 'Create new key', select 'JSON', and download the key (e.g., example-project-123456.json).
    // Share the Sheet with the Service Account: Open the JSON file, find the client_email (e.g., example-service-account@example-project.iam.gserviceaccount.com). In your Google Sheet, click 'Share', paste the client_email, set permission to 'Editor', and click 'Send' or 'Done'. Copy the JSON file to JsonFilePath and update JsonFileName in this config.
    // Test the Plugin: Update all config fields (SpreadsheetId, JsonFilePath, JsonFileName), restart the CS2 server, and test with css_gs_connect to confirm connection, then css_gsget and css_gsset 'test'. Common issues: 1) 'No permission' - ensure client_email is shared as Editor. 2) 'API key not found' - verify API is enabled and JSON file is correct. 3) 'Network error' - check server internet. 4) Reload fails - restart server due to CounterStrikeSharp bug.
    "SpreadsheetId": "example-spreadsheet-id-123456789",
    // The name of the sheet in the Google Sheet to interact with (e.g., 'ExampleSheet'). This is the tab name visible at the bottom of the Google Sheet.
    "SheetName": "ExampleSheet",
    // The cell in the sheet to interact with (e.g., 'B2' for cell B2). Combine with SheetName to form the range (e.g., 'ExampleSheet!B2').
    "CellName": "B2"
  },
  "CredentialsSettings": {
    // The directory path where the Google Sheets API credentials JSON file is stored. Ensure the path ends with a slash (/).
    "JsonFilePath": "/path/to/your/server/addons/counterstrikesharp/plugins/CS2GoogleSheetPlugin/",
    // The name of the Google Sheets API credentials JSON file (e.g., 'example-project-123456.json'). This file should be placed in the JsonFilePath directory.
    "JsonFileName": "example-project-123456.json"
  },
  "PluginSettings": {
    // The duration (in minutes) to cache the data retrieved from Google Sheets to reduce API calls. Set to 0 to disable caching.
    "CacheDurationMinutes": 2,
    // Enables or disables debug logging. When true, detailed logs (e.g., 'Executing command', 'Fetching data') are printed to the console. Set to false to reduce log output.
    "DebugLog": true
  }
}
```

### Example without Instructions
This example shows a typical configuration after setup, without the instructional comments:

```json
{
  "GoogleSheetSettings": {
    "SpreadsheetId": "1FghF4Thzj-bDxqWaQrelqNH8zfIKl0ErfvKijs9P3SM",
    "SheetName": "Sheet1",
    "CellName": "A1"
  },
  "CredentialsSettings": {
    "JsonFilePath": "/home/dathost/cs2_linux/game/csgo/addons/counterstrikesharp/plugins/CS2GoogleSheetPlugin/",
    "JsonFileName": "fluid-firefly-454703-e1-875afcc07446.json"
  },
  "PluginSettings": {
    "CacheDurationMinutes": 5,
    "DebugLog": false
  }
}
```

### Config Fields
- **GoogleSheetSettings**:
  - `SpreadsheetId`: The ID of your Google Sheet (found in the URL).
  - `SheetName`: The name of the sheet (e.g., `Sheet1`).
  - `CellName`: The cell to interact with (e.g., `B2`).
- **CredentialsSettings**:
  - `JsonFilePath`: Path to the Google Sheets API credentials JSON file (automatically set to the plugin directory if empty).
  - `JsonFileName`: Name of the credentials JSON file.
- **PluginSettings**:
  - `CacheDurationMinutes`: Cache duration for retrieved data (in minutes).
  - `DebugLog`: Enable/disable debug logging.

## Commands
- `css_gsget`: Get data from the specified cell.
- **Example Output**: `[CS2-plugin-googlesheet] - [Sheet1!A1] - <value>`
- `css_gsset "value"`: Set data in the specified cell.
- **Example**: `css_gsset "test value"`
- `css_gsadd "value"`: Append data to the specified cell.
- **Example**: `css_gsadd " appended"`
- `css_gsremove "value"`: Remove specific data from the specified cell.
- **Example**: `css_gsremove " appended"`
- `css_gs_connect`: Check connection to Google Sheets API.
- **Example Output**: `[GoogleSheetPlugin] Connection status: Connected to Google Sheets API.`

## Future Plans
This plugin is a starting point for integrating Google Sheets with CS2 servers. Future enhancements may include:

- **Multi-Cell Support**: Allow interaction with multiple cells or ranges (e.g., `A1:A10`) to store more complex data like player stats or match results.
- **Event-Driven Updates**: Automatically update Google Sheets based on in-game events (e.g., player kills, match end).
- **Data Visualization**: Generate charts or reports in Google Sheets based on CS2 server data.
- **Two-Way Sync**: Enable Google Sheets to send data back to the CS2 server (e.g., for dynamic server configurations or announcements).
- **Error Handling Improvements**: Add more detailed error messages and recovery mechanisms for network or API issues.
- **In-Memory Cache with Sync**: Maintain an in-memory cache of sheet tabs synced to `gs_adaptor.json` on a timer or event (like map end). Backups ensure data safety.
- **Structured Data Tabs**:
  - **LOGS**: Record server events (e.g., `[Time, SteamID, Action, Location, Details]`).
  - **STATS**: Track player stats (kills, deaths, playtime).
  - **INFO**: Store server info (e.g., rules, FAQs).
  - **ANNOUNCE**: Hold broadcast messages.
  - **ADMINS**: List permissions per SteamID.
- **Player Performance Monitoring**: Monitor player performance automatically. Hook into game events (deaths, connects, disconnects) to update the "STATS" tab with kills, deaths, playtime, and status. Stats persist across sessions.
- **Automated Announcements**: Use the "ANNOUNCE" tab to schedule and broadcast messages in-game.
- **Admin Permissions**: Use the "ADMINS" tab to manage permissions per SteamID, enabling role-based access control.
- **Logging System**: Implement a comprehensive logging system to record server events in the "LOGS" tab.
- **Timer Automation**: Sync data with Google Sheets on a timer (e.g., every 5 minutes) or triggered by events (e.g., map end).
- **Web Integration**: Host a website pulling sheet data for live stats, ban lists, or a server calendar.
- **Economy Link**: Tie stats to an in-game currency (e.g., kills earn points) tracked in Google Sheets.
- **Event System**: Use Google Sheets to schedule events (e.g., "Double XP at 8 PM") announced in-game.
- **Moderation Tools**: Ban or warn players via Google Sheets, with logs syncing back to enforce rules.
- **Customization**: Let admins define new tabs or commands in the config for unique server features.

## Contributing
Feel free to fork this repository, make improvements, and submit pull requests. If you encounter any issues or have suggestions, please open an issue on GitHub.

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
