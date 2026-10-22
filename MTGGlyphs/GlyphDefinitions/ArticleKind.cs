namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"a wolf", "werewolf", "a token": one kind of permanent with its optional article, as an item in a "that's a … or a …" list.</summary>
/// <exampledoc>Howlpack Resurgence</exampledoc>
/// <examplecapture>a wolf</examplecapture>
[Dependent]
public class ArticleKind : Glyph
{
    public override Nib[] Nibs => [Opt(Pattern("an?")), Prop(Kind)];

    public PermanentKind Kind { get; set; }
}
