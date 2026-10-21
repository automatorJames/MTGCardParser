namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Soulbond's pairing as a condition: "this creature is paired with another creature", "it's paired with a creature".</summary>
/// <exampledoc>Wingcrafter</exampledoc>
/// <examplecapture>this creature is paired with another creature</examplecapture>
[Dependent]
public class IsPaired : Glyph
{
    public override Nib[] Nibs => [Prop(Paired), Alt("is", "'s"), "paired with", Prop(Partner)];

    public PermanentPhrase Paired { get; set; }
    public IndefinitePermanent Partner { get; set; }
}
