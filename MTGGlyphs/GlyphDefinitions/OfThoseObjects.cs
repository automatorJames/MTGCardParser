namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"of those creatures", "of those cards", "of them" in "for each of those creatures" / "for each of them": counting objects named earlier in the line.</summary>
/// <exampledoc>Archon of Absolution</exampledoc>
/// <examplecapture>of those creatures</examplecapture>
[Dependent]
public class OfThoseObjects : Glyph
{
    public override Nib[] Nibs => ["of", Prop(Objects)];

    public OneOf<ThatCard, Them, ThatCardPhrase> Objects { get; set; }
}
