namespace MTGCardParser;
using System.Net;
using System.Text;

public static class HtmlReportGenerator
{
    private static string GetHeader(string title, bool isCardCoveragePage = false)
    {
        var sb = new StringBuilder();
        sb.Append($@"
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
            line-height: 24px;
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
            border-left: 5px solid;
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
            position: relative;
            padding: 0.1em 0.3em;
            border-radius: 3px;
            color: #1e1e1e;
            cursor: help;
        }}
        .unmatched-highlight {{
            background-color: #6980d1;
            color: #1e1e1e;
            padding: 0.1em 0.3em;
            border-radius: 3px;
        }}
        .highlight::after {{
            content: attr(data-title);
            position: absolute;
            left: 50%;
            bottom: 100%;
            transform: translateX(-50%);
            background: #333;
            color: white;
            padding: 4px 8px;
            border-radius: 4px;
            white-space: nowrap;
            opacity: 0;
            pointer-events: none;
            transition: opacity 0.05s;
            z-index: 999;
            margin-bottom: 4px;
        }}
        .highlight:hover::after {{
            opacity: 1;
        }}
        .copy-feedback {{
            position: fixed;
            background-color: #4CAF50;
            color: white;
            padding: 8px 16px;
            border-radius: 4px;
            z-index: 1000;
            opacity: 0;
            transition: opacity 0.5s;
            pointer-events: none;
            font-size: 0.9em;
        }}
");

        if (isCardCoveragePage)
        {
            // CSS rules are now written with tooltips being the default state (body.tooltips-enabled)
            // and superscripts being the opt-out state (when body does not have .tooltips-enabled).
            sb.Append(@"
        /* --- Card Coverage Page Specific Styles --- */
        
        /* State when tooltips are ON (DEFAULT) */
        body.tooltips-enabled #main-content td:nth-child(2) {
            line-height: 24px;
            padding-top: 12px;
        }
        body.tooltips-enabled .highlight-label {
            display: none;
        }
        body.tooltips-enabled .highlight:hover::after {
            display: block;
        }

        /* State when tooltips are OFF (superscripts are ON) */
        #main-content td:nth-child(2) {
            line-height: 1.8em;
            padding-top: 24px;
            transition: all 0.2s ease-in-out; /* Smooth transition for padding/line-height */
        }
        .highlight {
            cursor: default;
        }
        .highlight-label {
            position: absolute;
            bottom: 75%;
            left: 50%;
            transform: translateX(-50%);
            font-size: 0.6em;
            font-family: Verdana, Arial, sans-serif;
            font-weight: bold;
            white-space: nowrap;
            pointer-events: none;
            display: block;
        }
        .highlight::after {
            display: none;
        }

        /* --- Toggle Switch Styles --- */
        .toggle-switch-container {
            display: flex;
            align-items: center;
            margin-bottom: 1rem;
            background-color: #2a2d2e;
            padding: 10px 15px;
            border-radius: 4px;
            border: 1px solid #444;
            width: fit-content;
        }
        .toggle-switch-container label { margin-right: 10px; font-weight: bold; }
        .switch { position: relative; display: inline-block; width: 48px; height: 24px; }
        .switch input { opacity: 0; width: 0; height: 0; }
        .slider { position: absolute; cursor: pointer; top: 0; left: 0; right: 0; bottom: 0; background-color: #555; transition: .4s; border-radius: 24px; }
        .slider:before { position: absolute; content: ''; height: 18px; width: 18px; left: 3px; bottom: 3px; background-color: white; transition: .4s; border-radius: 50%; }
        input:checked + .slider { background-color: #569cd6; }
        input:checked + .slider:before { transform: translateX(24px); }
");
        }

        sb.Append(@"
    </style>
</head>
");

        // Add 'tooltips-enabled' class to body by default for the card coverage page
        string bodyClass = isCardCoveragePage ? " class=\"tooltips-enabled\"" : "";
        sb.Append($"<body{bodyClass}>");

        sb.Append($"<h1>{Encode(title)}</h1>");

        if (isCardCoveragePage)
        {
            sb.Append(@"
    <div class=""toggle-switch-container"">
        <label for=""tooltipToggle"">Enable Hover Tooltips:</label>
        <label class=""switch"">
            <input type=""checkbox"" id=""tooltipToggle"" checked>
            <span class=""slider""></span>
        </label>
    </div>");
        }

        sb.Append(@"<div id=""main-content"">");

        return sb.ToString();
    }

    private static string GetFooterAndScripts(bool isCardCoveragePage = false)
    {
        var sb = new StringBuilder();
        sb.Append(@"
    </div> <!-- close #main-content -->
    <script>
    document.addEventListener('DOMContentLoaded', () => {
        // --- Copy to Clipboard (Universal) ---
        document.body.addEventListener('click', (event) => {
            const target = event.target.closest('pre, td');
            if (!target) return;
            if (event.target.closest('a, button, input')) return;

            let textToCopy = (target.tagName === 'TD' && target.hasAttribute('data-original-text'))
                ? target.getAttribute('data-original-text')
                : target.innerText;
            
            textToCopy = textToCopy.trim();

            if (textToCopy) {
                navigator.clipboard.writeText(textToCopy).then(() => {
                    showCopyFeedback(event.clientX, event.clientY);
                }).catch(err => console.error('Failed to copy text: ', err));
            }
        });

        let feedbackDiv = null;
        let feedbackTimeout = null;
        function showCopyFeedback(x, y) {
            if (!feedbackDiv) {
                feedbackDiv = document.createElement('div');
                feedbackDiv.className = 'copy-feedback';
                feedbackDiv.textContent = 'Copied';
                document.body.appendChild(feedbackDiv);
            }
            clearTimeout(feedbackTimeout);
            feedbackDiv.style.left = `${x + 15}px`;
            feedbackDiv.style.top = `${y + 15}px`;
            feedbackDiv.style.opacity = '1';
            feedbackTimeout = setTimeout(() => { feedbackDiv.style.opacity = '0'; }, 1200);
        }
");

        if (isCardCoveragePage)
        {
            sb.Append(@"
        // --- Card Coverage Page Toggle ---
        const toggle = document.getElementById('tooltipToggle');
        if (toggle) {
            toggle.addEventListener('change', (event) => {
                // The 'checked' property reflects the new state of the checkbox
                document.body.classList.toggle('tooltips-enabled', event.target.checked);
            });
        }
");
        }

        sb.Append(@"
    });
    </script>
</body>
</html>");
        return sb.ToString();
    }


    public static string Generate(string title, Action<StringBuilder> contentBuilder, bool isCardCoveragePage = false)
    {
        var sb = new StringBuilder();
        sb.Append(GetHeader(title, isCardCoveragePage));
        contentBuilder(sb);
        sb.Append(GetFooterAndScripts(isCardCoveragePage));
        return sb.ToString();
    }

    public static string Encode(string text) => WebUtility.HtmlEncode(text);
}