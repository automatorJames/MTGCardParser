namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the top card of your library": the single top card of a library.</summary>
/// <exampledoc>Dark Confidant</exampledoc>
/// <examplecapture>the top card of your library</examplecapture>
[Dependent]
public class RevealedTopCard : Glyph
{
    public override Nib[] Nibs => ["the top card of", Prop(Whose), "library"];

    public Whose Whose { get; set; }
}
