namespace MTGCardParser.TokenTesting;
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
            transition: box-shadow 0.2s ease-in-out;
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
        tbody tr {{
            transition: box-shadow 0.2s ease-in-out;
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
            color: #dcdcaa;
            font-style: italic;
        }}
        pre.line-text {{
            color: #d4d4d4;
            padding: 1rem;
            cursor: default;
            line-height: 2.2em;
            margin-bottom: 1rem; /* Adjust to create space for details blocks */
        }}
        .nested-underline {{
            border-bottom: 1px solid; 
            border-color: var(--underline-color);
            padding-bottom: 4px; /* Base padding for the first level */
            cursor: pointer;
            transition: filter 0.2s ease-in-out;
        }}
        /* For each level of nesting, add more padding to push parent underlines down */
        .nested-underline .nested-underline {{ padding-bottom: 8px; }}
        .nested-underline .nested-underline .nested-underline {{ padding-bottom: 12px; }}
        .nested-underline .nested-underline .nested-underline .nested-underline {{ padding-bottom: 16px; }}
        .nested-underline .nested-underline .nested-underline .nested-underline .nested-underline {{ padding-bottom: 20px; }}

        .prop-capture {{
            border-top: 2px solid; 
            border-color: var(--prop-color);
            padding-top: 3px;
        }}
        .effect-details-block {{
            margin-left: 2rem;
            border-radius: 4px;
            padding-top: 0.5rem;
            margin-bottom: 0.5rem;
        }}
        .effect-details-block > h4 {{
            padding-left: 8px;
        }}
        .effect-details-block table {{
            width: auto;
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
        .effect-details-block tr:last-child td {{
            border-bottom: none;
        }}
        .property-child-block {{
            margin-left: 20px;
            padding-left: 15px;
            border-left: 2px solid #454545;
            margin-top: 15px;
            padding-top: 5px;
        }}
        .property-child-block h5 {{
            margin: 0 0 5px 0;
            padding-bottom: 3px;
            font-size: 0.95em;
            font-weight: bold;
            border-bottom: none;
            transition: box-shadow 0.2s ease-in-out;
            padding-left: 8px;
        }}
        .value-default {{ color: #b5cea8; }}
        .value-enum {{ color: #c586c0; }}
        .value-tokensegment {{ color: #ce9178; }}
        .value-empty {{ color: #808080; font-style: italic; }}

        /* Hover states */
        .nested-underline.highlight-active {{
            filter: brightness(1.6);
        }}
        
        .effect-details-block.highlight-active > h4 {{
             box-shadow: inset 3px 0 0 0 var(--highlight-color);
        }}

        tr.property-highlight, h5.property-highlight {{
            box-shadow: inset 3px 0 0 0 var(--highlight-color);
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
        /* Styles for Card Coverage Page omitted for brevity... */
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
            if (document.body.classList.contains('page-variable-capture') && !target.classList.contains('full-original-text')) return;
            // Prevent copy when interacting with highlightable elements
            if (event.target.closest('.line-text') || event.target.closest('.effect-details-block')) return; 
            let textToCopy = (target.tagName === 'TD' && target.hasAttribute('data-original-text')) ? target.getAttribute('data-original-text') : target.innerText.trim();
            if (textToCopy) {
                if (!event.shiftKey) textToCopy = textToCopy.toLowerCase();
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
            const mainContent = document.getElementById('main-content');
            let lastCaptureId = null;
            let lastPropertyName = null;

            mainContent.addEventListener('mouseover', (event) => {
                const target = event.target.closest('[data-capture-id]');
                const captureId = target ? target.dataset.captureId : null;
                const propertyName = target ? target.dataset.propertyName : null;

                if (captureId !== lastCaptureId) {
                    if (lastCaptureId) {
                        document.querySelectorAll(`[data-capture-id='${lastCaptureId}']`).forEach(el => el.classList.remove('highlight-active'));
                    }
                    if (captureId) {
                        const span = document.querySelector(`.nested-underline[data-capture-id='${captureId}']`);
                        const block = document.querySelector(`.effect-details-block[data-capture-id='${captureId}']`);
                        if (span && block) {
                            const mainColor = span.style.getPropertyValue('--underline-color');
                            span.classList.add('highlight-active');
                            block.classList.add('highlight-active');
                            block.style.setProperty('--highlight-color', mainColor);
                        }
                    }
                }

                if (propertyName !== lastPropertyName || captureId !== lastCaptureId) {
                     if (lastCaptureId) {
                        const oldBlock = document.querySelector(`.effect-details-block[data-capture-id='${lastCaptureId}']`);
                        if(oldBlock) {
                            oldBlock.querySelectorAll('.property-highlight').forEach(el => el.classList.remove('property-highlight'));
                        }
                     }
                     if (captureId && propertyName) {
                        const newBlock = document.querySelector(`.effect-details-block[data-capture-id='${captureId}']`);
                        if(newBlock) {
                            const propSpan = target.closest('.prop-capture');
                            if(propSpan) {
                                const propColor = propSpan.style.getPropertyValue('--prop-color');
                                newBlock.querySelectorAll(`[data-property-name='${propertyName}']`).forEach(el => {
                                    el.classList.add('property-highlight');
                                    el.style.setProperty('--highlight-color', propColor);
                                });
                            }
                        }
                     }
                }
                lastCaptureId = captureId;
                lastPropertyName = propertyName;
            });

            mainContent.addEventListener('mouseleave', () => {
                if (lastCaptureId) {
                    document.querySelectorAll('.highlight-active').forEach(el => el.classList.remove('highlight-active'));
                    const block = document.querySelector(`.effect-details-block[data-capture-id='${lastCaptureId}']`);
                    if(block) {
                       block.querySelectorAll('.property-highlight').forEach(el => el.classList.remove('property-highlight'));
                    }
                }
                lastCaptureId = null;
                lastPropertyName = null;
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