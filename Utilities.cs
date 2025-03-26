namespace CS2GoogleSheetPlugin
{
    public static class Utilities
    {
        public static string ExtractQuotedString(string input)
        {
            if (input.StartsWith("\"") && input.EndsWith("\"") && input.Length > 1)
            {
                return input.Substring(1, input.Length - 2);
            }
            return "";
        }
    }
}