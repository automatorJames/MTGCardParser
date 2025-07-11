namespace CardAnalysisInterface;

public static class Extensions
{
    public static string ToInlineStyle(this Dictionary<string, string> cssProperties) 
        => string.Join("; ", cssProperties.Select(x => x.Key + ": " + x.Value));


}
