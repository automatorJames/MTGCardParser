namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent being tapped for mana, as a condition: "a land is tapped for mana", "a basic land you control is tapped for mana", "a land is tapped for two or more mana".</summary>
/// <exampledoc>Contamination</exampledoc>
/// <examplecapture>a land is tapped for mana</examplecapture>
[Dependent]
public class PermanentTappedForMana : Glyph
{
    public override Nib[] Nibs => [Prop(Permanent), "is tapped for", Prop(Amount), "mana"];

    public IndefinitePermanent Permanent { get; set; }
    [Optional]
    public AtLeastCount Amount { get; set; }
}
