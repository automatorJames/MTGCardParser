using System.Net;

using System.Text;

public static class HtmlReportGenerator
{
    private static string GetHeader(string title)
    {
        return $@"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{Encode(title)}</title>
    <style>
        body {{
            background-color: #1e1e1e;
            color: #d4d4d4;
            font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif;
            margin: 0;
            padding: 2rem;
        }}
        h1, h2, h3 {{
            color: #569cd6;
            border-bottom: 1px solid #444;
            padding-bottom: 8px;
        }}
        table {{
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.4);
        }}
        th, td {{
            padding: 12px 15px;
            text-align: left;
            border: 1px solid #444;
            vertical-align: top;
        }}
        thead tr {{
            background-color: #2a2d2e;
            color: #d4d4d4;
            font-weight: bold;
        }}
        tbody tr:nth-of-type(even) {{
            background-color: #2a2d2e;
        }}
        tbody tr:hover {{
            background-color: #3a3d3e;
        }}
        .type-card {{
            background-color: #2a2d2e;
            border-left: 5px solid; /* color will be set inline */
            border-radius: 4px;
            padding: 1.5rem;
            margin-bottom: 2rem;
            box-shadow: 0 2px 8px rgba(0,0,0,0.5);
        }}
        .type-card h3 {{
            margin-top: 0;
            border-bottom: none;
        }}
        pre {{
            background-color: #1e1e1e;
            padding: 1rem;
            border-radius: 4px;
            border: 1px solid #444;
            white-space: pre-wrap;
            word-break: break-all;
        }}
        code {{
            font-family: Consolas, ""Courier New"", monospace;
            color: #9cdcfe;
        }}
        .highlight {{
            padding: 0.1em 0.3em;
            border-radius: 3px;
            color: #1e1e1e; /* Dark text for good contrast on light backgrounds */
            cursor: help;
        }}
        .unmatched-highlight {{
            background-color: #d16969; /* A muted red for unmatched spans */
            color: #1e1e1e;
            padding: 0.1em 0.3em;
            border-radius: 3px;
        }}
    </style>
</head>
<body>
    <h1>{Encode(title)}</h1>
";
    }

    private static string GetFooter() => "</body></html>";

    public static string Generate(string title, Action<StringBuilder> contentBuilder)
    {
        var sb = new StringBuilder();
        sb.Append(GetHeader(title));
        contentBuilder(sb);
        sb.Append(GetFooter());
        return sb.ToString();
    }

    public static string Encode(string text) => WebUtility.HtmlEncode(text);
}