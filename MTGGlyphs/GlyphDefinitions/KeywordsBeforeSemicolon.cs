namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Keywords set off by a semicolon from a later keyword that has reminder text: "flying;" in "flying; banding (…".</summary>
/// <exampledoc>Mesa Pegasus</exampledoc>
/// <examplecapture>flying;</examplecapture>
[Dependent]
public class KeywordsBeforeSemicolon : Glyph
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => [Prop(Keywords), ";"];

    public CardAbilityLine Keywords { get; set; }
}
