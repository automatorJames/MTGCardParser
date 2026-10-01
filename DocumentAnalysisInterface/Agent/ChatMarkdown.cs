using Markdig;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace DocumentAnalysisInterface.Agent;

/// <summary>
/// Renders the agent's Markdown replies as HTML for the chat pane. The replies quote the corpus, so nothing in them is
/// trusted: raw HTML is escaped, images are dropped and only ordinary links survive.
/// </summary>
public static class ChatMarkdown
{
    static readonly MarkdownPipeline _pipeline = new MarkdownPipelineBuilder()
        .UsePipeTables()
        .UseAutoLinks()
        .UseEmphasisExtras()
        .DisableHtml()
        .Build();

    public static string ToHtml(string markdown)
    {
        var document = Markdown.Parse(markdown ?? "", _pipeline);

        foreach (var link in document.Descendants<LinkInline>())
        {
            // An image would be fetched as soon as it is shown.
            link.IsImage = false;

            if (Uri.TryCreate(link.Url, UriKind.Absolute, out var uri) && uri.Scheme is "http" or "https")
            {
                link.GetAttributes().AddProperty("target", "_blank");
                link.GetAttributes().AddProperty("rel", "noopener noreferrer");
            }
            else
                link.Url = "#";
        }

        return document.ToHtml(_pipeline);
    }
}
