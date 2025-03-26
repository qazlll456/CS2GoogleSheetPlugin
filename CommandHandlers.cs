using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;

namespace CS2GoogleSheetPlugin
{
    public static class CommandHandlers
    {
        public static CommandInfo.CommandCallback OnGoogleSheetGetCommand(GoogleSheetPlugin plugin)
        {
            return (player, command) =>
            {
                if (plugin.DebugLog)
                {
                    Server.PrintToConsole("[GoogleSheetPlugin] Executing css_gsget command...");
                }
                if (plugin.SheetsService == null)
                {
                    if (plugin.DebugLog)
                    {
                        Server.PrintToConsole("[GoogleSheetPlugin] Error: Google Sheets API service not initialized.");
                    }
                    return;
                }

                try
                {
                    if (string.IsNullOrEmpty(plugin.SpreadsheetId))
                    {
                        if (plugin.DebugLog)
                        {
                            Server.PrintToConsole("[GoogleSheetPlugin] Error: Invalid spreadsheet ID.");
                        }
                        return;
                    }

                    if (plugin.Cache.TryGetValue(plugin.Range, out var cachedData) && (DateTime.Now - cachedData.Timestamp) < plugin.CacheDuration)
                    {
                        if (plugin.DebugLog)
                        {
                            Server.PrintToConsole("[GoogleSheetPlugin] Retrieved data from cache.");
                            Server.PrintToConsole($"[CS2-plugin-googlesheet] - [{plugin.Range}] - {cachedData.Value}");
                        }
                        return;
                    }

                    if (plugin.DebugLog)
                    {
                        Server.PrintToConsole($"[GoogleSheetPlugin] Fetching data from spreadsheet {plugin.SpreadsheetId}, range: {plugin.Range}...");
                    }
                    SpreadsheetsResource.ValuesResource.GetRequest request =
                        plugin.SheetsService.Spreadsheets.Values.Get(plugin.SpreadsheetId, plugin.Range);

                    ValueRange response = request.Execute();
                    string value = (response.Values != null && response.Values.Count > 0 && response.Values[0].Count > 0)
                        ? response.Values[0][0]?.ToString() ?? "No data found"
                        : "No data found";

                    plugin.Cache[plugin.Range] = (value, DateTime.Now);

                    if (plugin.DebugLog)
                    {
                        Server.PrintToConsole("[GoogleSheetPlugin] Received Google Sheets API response.");
                        Server.PrintToConsole($"[CS2-plugin-googlesheet] - [{plugin.Range}] - {value}");
                    }
                }
                catch (Google.GoogleApiException ex)
                {
                    if (plugin.DebugLog)
                    {
                        Server.PrintToConsole($"[GoogleSheetPlugin] Google API error: {ex.Message}");
                        if (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            Server.PrintToConsole("[GoogleSheetPlugin] Error: Spreadsheet or sheet not found.");
                        }
                        else if (ex.HttpStatusCode == System.Net.HttpStatusCode.Forbidden)
                        {
                            Server.PrintToConsole("[GoogleSheetPlugin] Error: No permission to access spreadsheet.");
                        }
                    }
                }
                catch (System.Net.Http.HttpRequestException ex)
                {
                    if (plugin.DebugLog)
                    {
                        Server.PrintToConsole($"[GoogleSheetPlugin] Network error: {ex.Message}");
                        Server.PrintToConsole("[GoogleSheetPlugin] Please check network connection or try again later.");
                    }
                }
                catch (Exception ex)
                {
                    if (plugin.DebugLog)
                    {
                        Server.PrintToConsole($"[GoogleSheetPlugin] Error fetching data: {ex.Message}");
                    }
                }
            };
        }

        public static CommandInfo.CommandCallback OnGoogleSheetSetCommand(GoogleSheetPlugin plugin)
        {
            return (player, command) =>
            {
                if (plugin.DebugLog)
                {
                    Server.PrintToConsole("[GoogleSheetPlugin] Executing css_gsset command...");
                }
                if (plugin.SheetsService == null)
                {
                    if (plugin.DebugLog)
                    {
                        Server.PrintToConsole("[GoogleSheetPlugin] Error: Google Sheets API service not initialized.");
                    }
                    return;
                }

                if (command.ArgCount < 2 || string.IsNullOrWhiteSpace(command.ArgString))
                {
                    if (plugin.DebugLog)
                    {
                        Server.PrintToConsole("[GoogleSheetPlugin] Usage: css_gsset \"value\"");
                    }
                    return;
                }

                string input = command.ArgString.Trim();
                string newValue = Utilities.ExtractQuotedString(input);
                if (string.IsNullOrEmpty(newValue) && input != "\"\"")
                {
                    if (plugin.DebugLog)
                    {
                        Server.PrintToConsole("[GoogleSheetPlugin] Usage: css_gsset \"value\" (use quotes for the value)");
                    }
                    return;
                }

                try
                {
                    if (string.IsNullOrEmpty(plugin.SpreadsheetId))
                    {
                        if (plugin.DebugLog)
                        {
                            Server.PrintToConsole("[GoogleSheetPlugin] Error: Invalid spreadsheet ID.");
                        }
                        return;
                    }

                    if (plugin.DebugLog)
                    {
                        Server.PrintToConsole($"[GoogleSheetPlugin] Setting data in spreadsheet {plugin.SpreadsheetId}, range: {plugin.Range}, value: {newValue}...");
                    }
                    ValueRange valueRange = new ValueRange
                    {
                        Values = new List<IList<object>> { new List<object> { newValue } }
                    };

                    SpreadsheetsResource.ValuesResource.UpdateRequest request =
                        plugin.SheetsService.Spreadsheets.Values.Update(valueRange, plugin.SpreadsheetId, plugin.Range);
                    request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;

                    UpdateValuesResponse response = request.Execute();

                    plugin.Cache.Remove(plugin.Range);

                    if (plugin.DebugLog)
                    {
                        Server.PrintToConsole("[GoogleSheetPlugin] Successfully set Google Sheets data.");
                    }
                }
                catch (Google.GoogleApiException ex)
                {
                    if (plugin.DebugLog)
                    {
                        Server.PrintToConsole($"[GoogleSheetPlugin] Google API error: {ex.Message}");
                        if (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            Server.PrintToConsole("[GoogleSheetPlugin] Error: Spreadsheet or sheet not found.");
                        }
                        else if (ex.HttpStatusCode == System.Net.HttpStatusCode.Forbidden)
                        {
                            Server.PrintToConsole("[GoogleSheetPlugin] Error: No permission to write to spreadsheet.");
                        }
                    }
                }
                catch (System.Net.Http.HttpRequestException ex)
                {
                    if (plugin.DebugLog)
                    {
                        Server.PrintToConsole($"[GoogleSheetPlugin] Network error: {ex.Message}");
                        Server.PrintToConsole("[GoogleSheetPlugin] Please check network connection or try again later.");
                    }
                }
                catch (Exception ex)
                {
                    if (plugin.DebugLog)
                    {
                        Server.PrintToConsole($"[GoogleSheetPlugin] Error setting data: {ex.Message}");
                    }
                }
            };
        }

        public static CommandInfo.CommandCallback OnGoogleSheetAddCommand(GoogleSheetPlugin plugin)
        {
            return (player, command) =>
            {
                if (plugin.DebugLog)
                {
                    Server.PrintToConsole("[GoogleSheetPlugin] Executing css_gsadd command...");
                }
                if (plugin.SheetsService == null)
                {
                    if (plugin.DebugLog)
                    {
                        Server.PrintToConsole("[GoogleSheetPlugin] Error: Google Sheets API service not initialized.");
                    }
                    return;
                }

                if (command.ArgCount < 2 || string.IsNullOrWhiteSpace(command.ArgString))
                {
                    if (plugin.DebugLog)
                    {
                        Server.PrintToConsole("[GoogleSheetPlugin] Usage: css_gsadd \"value\"");
                    }
                    return;
                }

                string input = command.ArgString.Trim();
                string appendValue = Utilities.ExtractQuotedString(input);
                if (string.IsNullOrEmpty(appendValue) && input != "\"\"")
                {
                    if (plugin.DebugLog)
                    {
                        Server.PrintToConsole("[GoogleSheetPlugin] Usage: css_gsadd \"value\" (use quotes for the value)");
                    }
                    return;
                }

                try
                {
                    if (string.IsNullOrEmpty(plugin.SpreadsheetId))
                    {
                        if (plugin.DebugLog)
                        {
                            Server.PrintToConsole("[GoogleSheetPlugin] Error: Invalid spreadsheet ID.");
                        }
                        return;
                    }

                    SpreadsheetsResource.ValuesResource.GetRequest getRequest =
                        plugin.SheetsService.Spreadsheets.Values.Get(plugin.SpreadsheetId, plugin.Range);
                    ValueRange getResponse = getRequest.Execute();
                    string currentValue = (getResponse.Values != null && getResponse.Values.Count > 0 && getResponse.Values[0].Count > 0)
                        ? getResponse.Values[0][0]?.ToString() ?? ""
                        : "";

                    string newValue = currentValue + appendValue;

                    ValueRange valueRange = new ValueRange
                    {
                        Values = new List<IList<object>> { new List<object> { newValue } }
                    };

                    SpreadsheetsResource.ValuesResource.UpdateRequest updateRequest =
                        plugin.SheetsService.Spreadsheets.Values.Update(valueRange, plugin.SpreadsheetId, plugin.Range);
                    updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;

                    UpdateValuesResponse updateResponse = updateRequest.Execute();

                    plugin.Cache.Remove(plugin.Range);

                    if (plugin.DebugLog)
                    {
                        Server.PrintToConsole($"[GoogleSheetPlugin] Successfully appended data to {plugin.Range}, new value: {newValue}");
                    }
                }
                catch (Google.GoogleApiException ex)
                {
                    if (plugin.DebugLog)
                    {
                        Server.PrintToConsole($"[GoogleSheetPlugin] Google API error: {ex.Message}");
                        if (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            Server.PrintToConsole("[GoogleSheetPlugin] Error: Spreadsheet or sheet not found.");
                        }
                        else if (ex.HttpStatusCode == System.Net.HttpStatusCode.Forbidden)
                        {
                            Server.PrintToConsole("[GoogleSheetPlugin] Error: No permission to write to spreadsheet.");
                        }
                    }
                }
                catch (System.Net.Http.HttpRequestException ex)
                {
                    if (plugin.DebugLog)
                    {
                        Server.PrintToConsole($"[GoogleSheetPlugin] Network error: {ex.Message}");
                        Server.PrintToConsole("[GoogleSheetPlugin] Please check network connection or try again later.");
                    }
                }
                catch (Exception ex)
                {
                    if (plugin.DebugLog)
                    {
                        Server.PrintToConsole($"[GoogleSheetPlugin] Error appending data: {ex.Message}");
                    }
                }
            };
        }

        public static CommandInfo.CommandCallback OnGoogleSheetRemoveCommand(GoogleSheetPlugin plugin)
        {
            return (player, command) =>
            {
                if (plugin.DebugLog)
                {
                    Server.PrintToConsole("[GoogleSheetPlugin] Executing css_gsremove command...");
                }
                if (plugin.SheetsService == null)
                {
                    if (plugin.DebugLog)
                    {
                        Server.PrintToConsole("[GoogleSheetPlugin] Error: Google Sheets API service not initialized.");
                    }
                    return;
                }

                if (command.ArgCount < 2 || string.IsNullOrWhiteSpace(command.ArgString))
                {
                    if (plugin.DebugLog)
                    {
                        Server.PrintToConsole("[GoogleSheetPlugin] Usage: css_gsremove \"value\"");
                    }
                    return;
                }

                string input = command.ArgString.Trim();
                string removeValue = Utilities.ExtractQuotedString(input);
                if (string.IsNullOrEmpty(removeValue) && input != "\"\"")
                {
                    if (plugin.DebugLog)
                    {
                        Server.PrintToConsole("[GoogleSheetPlugin] Usage: css_gsremove \"value\" (use quotes for the value)");
                    }
                    return;
                }

                try
                {
                    if (string.IsNullOrEmpty(plugin.SpreadsheetId))
                    {
                        if (plugin.DebugLog)
                        {
                            Server.PrintToConsole("[GoogleSheetPlugin] Error: Invalid spreadsheet ID.");
                        }
                        return;
                    }

                    SpreadsheetsResource.ValuesResource.GetRequest getRequest =
                        plugin.SheetsService.Spreadsheets.Values.Get(plugin.SpreadsheetId, plugin.Range);
                    ValueRange getResponse = getRequest.Execute();
                    string currentValue = (getResponse.Values != null && getResponse.Values.Count > 0 && getResponse.Values[0].Count > 0)
                        ? getResponse.Values[0][0]?.ToString() ?? ""
                        : "";

                    string newValue = currentValue.Replace(removeValue, "");

                    ValueRange valueRange = new ValueRange
                    {
                        Values = new List<IList<object>> { new List<object> { newValue } }
                    };

                    SpreadsheetsResource.ValuesResource.UpdateRequest updateRequest =
                        plugin.SheetsService.Spreadsheets.Values.Update(valueRange, plugin.SpreadsheetId, plugin.Range);
                    updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;

                    UpdateValuesResponse updateResponse = updateRequest.Execute();

                    plugin.Cache.Remove(plugin.Range);

                    if (plugin.DebugLog)
                    {
                        Server.PrintToConsole($"[GoogleSheetPlugin] Successfully removed data from {plugin.Range}, new value: {newValue}");
                    }
                }
                catch (Google.GoogleApiException ex)
                {
                    if (plugin.DebugLog)
                    {
                        Server.PrintToConsole($"[GoogleSheetPlugin] Google API error: {ex.Message}");
                        if (ex.HttpStatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            Server.PrintToConsole("[GoogleSheetPlugin] Error: Spreadsheet or sheet not found.");
                        }
                        else if (ex.HttpStatusCode == System.Net.HttpStatusCode.Forbidden)
                        {
                            Server.PrintToConsole("[GoogleSheetPlugin] Error: No permission to write to spreadsheet.");
                        }
                    }
                }
                catch (System.Net.Http.HttpRequestException ex)
                {
                    if (plugin.DebugLog)
                    {
                        Server.PrintToConsole($"[GoogleSheetPlugin] Network error: {ex.Message}");
                        Server.PrintToConsole("[GoogleSheetPlugin] Please check network connection or try again later.");
                    }
                }
                catch (Exception ex)
                {
                    if (plugin.DebugLog)
                    {
                        Server.PrintToConsole($"[GoogleSheetPlugin] Error removing data: {ex.Message}");
                    }
                }
            };
        }

        public static CommandInfo.CommandCallback OnGoogleSheetConnectCommand(GoogleSheetPlugin plugin)
        {
            return (player, command) =>
            {
                if (plugin.DebugLog)
                {
                    Server.PrintToConsole("[GoogleSheetPlugin] Executing css_gs_connect command...");
                }
                if (plugin.SheetsService == null)
                {
                    if (plugin.DebugLog)
                    {
                        Server.PrintToConsole("[GoogleSheetPlugin] Connection status: Not connected to Google Sheets API.");
                    }
                }
                else
                {
                    if (plugin.DebugLog)
                    {
                        Server.PrintToConsole("[GoogleSheetPlugin] Connection status: Connected to Google Sheets API.");
                        Server.PrintToConsole("[GoogleSheetPlugin] Google Sheet name: CS2-plugin-googlesheet");
                    }
                }
            };
        }
    }
}