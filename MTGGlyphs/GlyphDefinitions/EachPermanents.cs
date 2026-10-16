namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"each creature you control", "each other creature", "each attacking creature an opponent controls": every permanent of a kind, one at a time.</summary>
/// <exampledoc>Abzan Ascendancy</exampledoc>
/// <examplecapture>each creature you control</examplecapture>
[Dependent]
public class EachPermanents : Glyph
{
    public override Nib[] Nibs => ["each", Prop(Other), Prop(Permanents)];

    [RegexPattern("other")]
    public bool Other { get; set; }
    public Permanents Permanents { get; set; }
}
