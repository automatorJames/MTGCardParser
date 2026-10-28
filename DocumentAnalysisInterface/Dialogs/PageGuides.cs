namespace DocumentAnalysisInterface.Dialogs;

/// <summary>The pages that have a guide (see <see cref="PageGuideDialog"/>), by route, and what each is called.</summary>
public static class PageGuides
{
    public const string CorpusCaptures = "/";
    public const string GlyphRegex = "glyph-regex";
    public const string EchoTrees = "echo-trees";
    public const string GrammarTools = "grammar-tools";
    public const string Lowered = "lowered";

    public static bool HasGuide(string route) =>
        route is CorpusCaptures or GlyphRegex or EchoTrees or GrammarTools or Lowered;

    /// <summary>The page's name, as its tab reads - for the lowered page, the engine's own name is shown by the layout instead.</summary>
    public static string NameOf(string route) =>
        route switch
        {
            CorpusCaptures => "Corpus Captures",
            GlyphRegex => "Glyph Regex",
            EchoTrees => "Echo Trees",
            GrammarTools => "Grammar Tools",
            Lowered => "Lowered",
            _ => "",
        };
}
