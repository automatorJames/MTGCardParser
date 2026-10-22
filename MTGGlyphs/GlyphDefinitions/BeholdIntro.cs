namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The opening of behold's reminder text, naming what's beheld: "to behold a dragon,".</summary>
/// <exampledoc>Sarkhan, Dragon Ascendant</exampledoc>
/// <examplecapture>to behold a dragon,</examplecapture>
[Dependent]
public class BeholdIntro : Glyph
{
    public override Nib[] Nibs => ["to behold", Prop(Kind), ","];

    public ArticleKind Kind { get; set; }
}
