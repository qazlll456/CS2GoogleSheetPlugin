using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace CS2GoogleSheetPlugin
{
    public class GoogleSheetPlugin : BasePlugin
    {
        public override string ModuleName => "CS2GoogleSheetPlugin";
        public override string ModuleVersion => "1.0.0";
        public override string ModuleAuthor => "qazlll456 from HK with xAI assistance";
        public override string ModuleDescription => "A plugin to interact with Google Sheets for CS2, allowing get, set, add, and remove operations on a specified cell.";
        private SheetsService? _sheetsService;
        public GoogleSheetPluginConfig? Config { get; set; }
        private string range => Config != null ? $"{Config.GoogleSheetSettings.SheetName}!{Config.GoogleSheetSettings.CellName}" : throw new InvalidOperationException("Config is not initialized.");

        // Cache
        private Dictionary<string, (string Value, DateTime Timestamp)> _cache = new Dictionary<string, (string, DateTime)>();

        public override void Load(bool hotReload)
        {
            // Manually load or generate the config file
            string configDir = Path.Combine(ModuleDirectory, "..", "..", "configs", "plugins", "CS2GoogleSheetPlugin");
            string configPath = Path.Combine(configDir, "CS2GoogleSheetPlugin.json");

            Console.WriteLine($"[GoogleSheetPlugin] Config directory: {configDir}");
            Console.WriteLine($"[GoogleSheetPlugin] Config path: {configPath}");

            try
            {
                Directory.CreateDirectory(configDir);
                Console.WriteLine($"[GoogleSheetPlugin] Config directory created or already exists.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GoogleSheetPlugin] Failed to create config directory: {ex.Message}");
                throw;
            }

            try
            {
                if (File.Exists(configPath))
                {
                    Console.WriteLine($"[GoogleSheetPlugin] Config file exists, loading...");
                    Config = JsonSerializer.Deserialize<GoogleSheetPluginConfig>(File.ReadAllText(configPath));
                    Console.WriteLine($"[GoogleSheetPlugin] Config loaded successfully.");
                }
                else
                {
                    Console.WriteLine($"[GoogleSheetPlugin] Config file does not exist, creating new config...");
                    Config = new GoogleSheetPluginConfig();
                    File.WriteAllText(configPath, JsonSerializer.Serialize(Config, new JsonSerializerOptions { WriteIndented = true }));
                    Console.WriteLine($"[GoogleSheetPlugin] New config file created at {configPath}.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GoogleSheetPlugin] Failed to load or create config file: {ex.Message}");
                throw;
            }

            if (Config == null)
            {
                throw new InvalidOperationException("Failed to load or create configuration. Ensure the config file is valid.");
            }

            // Set the JsonFilePath dynamically if it's not set in the config
            if (string.IsNullOrEmpty(Config.CredentialsSettings.JsonFilePath))
            {
                Config.CredentialsSettings.JsonFilePath = ModuleDirectory;
                Console.WriteLine($"[GoogleSheetPlugin] JsonFilePath set to: {Config.CredentialsSettings.JsonFilePath}");
                // Save the updated config
                File.WriteAllText(configPath, JsonSerializer.Serialize(Config, new JsonSerializerOptions { WriteIndented = true }));
            }

            if (Config.PluginSettings.DebugLog)
            {
                Console.WriteLine("[GoogleSheetPlugin] Starting plugin load...");
            }
            try
            {
                string[] scopes = { SheetsService.Scope.Spreadsheets };
                GoogleCredential credential;

                if (Config.PluginSettings.DebugLog)
                {
                    Console.WriteLine("[GoogleSheetPlugin] Loading service account credentials...");
                }
                string jsonFilePath = System.IO.Path.Combine(Config.CredentialsSettings.JsonFilePath, Config.CredentialsSettings.JsonFileName);
                Console.WriteLine($"[GoogleSheetPlugin] Attempting to load JSON file from: {jsonFilePath}");
                using (var stream = new System.IO.FileStream(jsonFilePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream).CreateScoped(scopes);
                }

                if (Config.PluginSettings.DebugLog)
                {
                    Console.WriteLine("[GoogleSheetPlugin] Initializing Google Sheets API...");
                }
                _sheetsService = new SheetsService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "CS2 Google Sheet Plugin",
                });

                if (Config.PluginSettings.DebugLog)
                {
                    Console.WriteLine("[GoogleSheetPlugin] Successfully initialized Google Sheets API.");
                }
            }
            catch (Exception ex)
            {
                if (Config.PluginSettings.DebugLog)
                {
                    Console.WriteLine($"[GoogleSheetPlugin] Failed to initialize Google Sheets API: {ex.Message}");
                }
            }

            if (Config.PluginSettings.DebugLog)
            {
                Console.WriteLine("[GoogleSheetPlugin] Registering command css_gsget...");
            }
            AddCommand("css_gsget", "Get data from Google Sheet", CommandHandlers.OnGoogleSheetGetCommand(this));
            if (Config.PluginSettings.DebugLog)
            {
                Console.WriteLine("[GoogleSheetPlugin] Registering command css_gsset...");
            }
            AddCommand("css_gsset", "Set data in Google Sheet", CommandHandlers.OnGoogleSheetSetCommand(this));
            if (Config.PluginSettings.DebugLog)
            {
                Console.WriteLine("[GoogleSheetPlugin] Registering command css_gsadd...");
            }
            AddCommand("css_gsadd", "Append data to Google Sheet", CommandHandlers.OnGoogleSheetAddCommand(this));
            if (Config.PluginSettings.DebugLog)
            {
                Console.WriteLine("[GoogleSheetPlugin] Registering command css_gsremove...");
            }
            AddCommand("css_gsremove", "Remove data from Google Sheet", CommandHandlers.OnGoogleSheetRemoveCommand(this));
            if (Config.PluginSettings.DebugLog)
            {
                Console.WriteLine("[GoogleSheetPlugin] Registering command css_gs_connect...");
            }
            AddCommand("css_gs_connect", "Check Google Sheet connection", CommandHandlers.OnGoogleSheetConnectCommand(this));
            if (Config.PluginSettings.DebugLog)
            {
                Console.WriteLine("[GoogleSheetPlugin] Plugin load completed.");
            }
        }

        // Properties for CommandHandlers to access
        public SheetsService? SheetsService => _sheetsService;
        public string SpreadsheetId => Config?.GoogleSheetSettings.SpreadsheetId ?? throw new InvalidOperationException("Config is not initialized.");
        public string Range => range;
        public Dictionary<string, (string Value, DateTime Timestamp)> Cache => _cache;
        public TimeSpan CacheDuration => TimeSpan.FromMinutes(Config?.PluginSettings.CacheDurationMinutes ?? throw new InvalidOperationException("Config is not initialized."));
        public bool DebugLog => Config?.PluginSettings.DebugLog ?? throw new InvalidOperationException("Config is not initialized.");
    }
}