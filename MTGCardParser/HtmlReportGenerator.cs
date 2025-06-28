// Splice in this updated version of HtmlReportGenerator.cs

namespace MTGCardParser;
using System.Net;
using System.Text;

public static class HtmlReportGenerator
{
    private static string GetHeader(string title, bool isCardCoveragePage = false, bool isVariableCapturePage = false)
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
        h1, h2, h3, h4, h5 {{
            color: #569cd6;
            border-bottom: 1px solid #444;
            padding-bottom: 8px;
        }}
        h4 {{
            color: #9cdcfe;
            border-bottom-style: dashed;
            margin-top: 1.5rem;
            margin-bottom: 0.5rem;
            transition: filter 0.2s ease-in-out;
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
            cursor: copy;
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
            cursor: copy;
        }}
        code {{
            font-family: Consolas, ""Courier New"", monospace;
            color: #9cdcfe;
        }}
        
        /* --- Card Variable Capture Page Styles --- */
        .card-capture-block {{
            background-color: #252526;
            border: 1px solid #444;
            border-radius: 4px;
            padding: 1.5rem;
            margin-bottom: 2rem;
            box-shadow: 0 2px 8px rgba(0,0,0,0.5);
        }}
        .card-capture-block h2 {{
            margin-top: 0;
        }}
        .line-label {{
            font-size: 0.9em;
            color: #888;
            margin-top: 2rem;
            margin-bottom: 0.5rem;
            border-bottom: 1px dashed #444;
        }}
        pre.full-original-text {{
            color: #dcdcaa; /* VS Code string color */
            font-style: italic;
        }}
        pre.line-text {{
            color: #d4d4d4;
            padding-bottom: 1.5rem;
            cursor: default; /* No copy cursor */
        }}
        .captured-text {{
            border-bottom: 2px solid; /* Color will be set inline */
            padding-bottom: 2px;
            cursor: pointer;
            transition: filter 0.2s ease-in-out;
        }}
        .effect-details-block {{
            margin-left: 2rem;
        }}
        .effect-details-block table {{
            width: auto; /* Allow table to expand */
            min-width: 600px;
            margin-top: 10px;
            box-shadow: none;
            border: 1px solid #444;
        }}
        .effect-details-block th, .effect-details-block td {{
            padding: 6px 12px;
            font-size: 0.9em;
            border: none;
            border-bottom: 1px solid #444;
            cursor: default;
        }}
        .effect-details-block th:nth-child(1), .effect-details-block td:nth-child(1) {{ width: 320px; }}
        .effect-details-block th:nth-child(2), .effect-details-block td:nth-child(2) {{ width: 240px; }}
        .effect-details-block tr:last-child td {{
            border-bottom: none;
        }}
        .value-default {{ color: #b5cea8; }}
        .value-enum {{ color: #c586c0; }}
        .value-tokensegment {{ color: #ce9178; }}
        .value-empty {{ color: #808080; font-style: italic; }}
        .highlight-active {{
            filter: brightness(1.6);
        }}

        .highlight {{
            position: relative; padding: 0.1em 0.3em; border-radius: 3px;
            color: #1e1e1e; cursor: help;
        }}
        .unmatched-highlight {{
            background-color: #6980d1; color: #1e1e1e; padding: 0.1em 0.3em; border-radius: 3px;
        }}
        .highlight::after {{
            content: attr(data-title); position: absolute; left: 50%; bottom: 100%; transform: translateX(-50%);
            background: #333; color: white; padding: 4px 8px; border-radius: 4px; white-space: nowrap;
            opacity: 0; pointer-events: none; transition: opacity 0.05s; z-index: 999; margin-bottom: 4px;
        }}
        .highlight:hover::after {{ opacity: 1; }}
        .copy-feedback {{
            position: fixed; background-color: #4CAF50; color: white; padding: 8px 16px;
            border-radius: 4px; z-index: 1000; opacity: 0; transition: opacity 0.5s;
            pointer-events: none; font-size: 0.9em;
        }}
");

        if (isCardCoveragePage)
        {
            sb.Append(@"
        body.tooltips-enabled #main-content td:nth-child(2) { line-height: 24px; padding-top: 12px; }
        body.tooltips-enabled .highlight-label { display: none; }
        body.tooltips-enabled .highlight { cursor: help; }
        body.tooltips-enabled .highlight:hover::after { display: block; }
        #main-content td:nth-child(2) { line-height: 1.8em; padding-top: 24px; transition: all 0.2s ease-in-out; }
        .highlight { cursor: default; }
        .highlight-label { position: absolute; bottom: 75%; left: 50%; transform: translateX(-50%); font-size: 0.7em; font-family: Verdana, Arial, sans-serif; font-weight: bold; white-space: nowrap; pointer-events: none; display: block; }
        .highlight::after { display: none; }
        .toggle-switch-container { display: flex; align-items: center; margin-bottom: 1rem; background-color: #2a2d2e; padding: 10px 15px; border-radius: 4px; border: 1px solid #444; width: fit-content; }
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
        var bodyClasses = new List<string>();
        if (isCardCoveragePage) bodyClasses.Add("tooltips-enabled");
        if (isVariableCapturePage) bodyClasses.Add("page-variable-capture");

        string bodyClassAttr = bodyClasses.Any() ? $" class=\"{string.Join(" ", bodyClasses)}\"" : "";
        sb.Append($"<body{bodyClassAttr}>");

        sb.Append($"<h1>{Encode(title)}</h1>");

        if (isCardCoveragePage)
        {
            sb.Append(@"
    <div class=""toggle-switch-container"">
        <label for=""tooltipToggle"">Enable Hover Tooltips:</label>
        <label class=""switch""> <input type=""checkbox"" id=""tooltipToggle"" checked> <span class=""slider""></span> </label>
    </div>");
        }

        sb.Append(@"<div id=""main-content"">");

        return sb.ToString();
    }

    private static string GetFooterAndScripts(bool isCardCoveragePage = false)
    {
        var sb = new StringBuilder();
        sb.Append(@"
    </div>
    <script>
    document.addEventListener('DOMContentLoaded', () => {
        // --- Click to Copy Logic ---
        document.body.addEventListener('click', (event) => {
            let target = event.target.closest('pre, td');
            if (!target) return;

            // On variable capture page, only allow copy from the main text block
            if (document.body.classList.contains('page-variable-capture') && !target.classList.contains('full-original-text')) {
                return;
            }
            if (event.target.closest('a, button, input, .effect-details-block')) return;

            let textToCopy = (target.tagName === 'TD' && target.hasAttribute('data-original-text')) ? target.getAttribute('data-original-text') : target.innerText.trim();
            
            if (textToCopy) {
                // If the shift key is not held, convert text to lowercase before copying.
                if (!event.shiftKey) {
                    textToCopy = textToCopy.toLowerCase();
                }
                navigator.clipboard.writeText(textToCopy).then(() => showCopyFeedback(event.clientX, event.clientY)).catch(err => console.error('Failed to copy text: ', err));
            }
        });

        let feedbackDiv = null, feedbackTimeout = null;
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

        // --- Variable Capture Page Hover Logic ---
        if (document.body.classList.contains('page-variable-capture')) {
            let lastHoverId = null;
            document.getElementById('main-content').addEventListener('mouseover', (event) => {
                const target = event.target.closest('[data-capture-id]');
                const currentId = target ? target.dataset.captureId : null;
                
                if (currentId !== lastHoverId) {
                    // Remove old highlights
                    if (lastHoverId) {
                        document.querySelectorAll(`[data-capture-id='${lastHoverId}']`).forEach(el => el.classList.remove('highlight-active'));
                    }
                    // Add new highlights
                    if (currentId) {
                        document.querySelectorAll(`[data-capture-id='${currentId}']`).forEach(el => el.classList.add('highlight-active'));
                    }
                }
                lastHoverId = currentId;
            });
        }
");

        if (isCardCoveragePage)
        {
            sb.Append(@"
        // --- Card Coverage Page Toggle ---
        const toggle = document.getElementById('tooltipToggle');
        if (toggle) toggle.addEventListener('change', (event) => document.body.classList.toggle('tooltips-enabled', event.target.checked));
");
        }

        sb.Append(@"
    });
    </script>
</body>
</html>");
        return sb.ToString();
    }

    public static string Generate(string title, Action<StringBuilder> contentBuilder, bool isCardCoveragePage = false, bool isVariableCapturePage = false)
    {
        var sb = new StringBuilder();
        sb.Append(GetHeader(title, isCardCoveragePage, isVariableCapturePage));
        contentBuilder(sb);
        sb.Append(GetFooterAndScripts(isCardCoveragePage));
        return sb.ToString();
    }

    public static string Encode(string text) => WebUtility.HtmlEncode(text);
}