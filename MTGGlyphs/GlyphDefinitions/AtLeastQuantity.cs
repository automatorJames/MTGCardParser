namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"at least four", "at least three": a count at or above a number, said before it.</summary>
/// <exampledoc>Locthwain Paladin</exampledoc>
/// <examplecapture>at least three</examplecapture>
[Dependent]
public class AtLeastQuantity : Glyph
{
    public override Nib[] Nibs => ["at least", Prop(Count)];

    public Quantity Count { get; set; }
}
