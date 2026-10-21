namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An amount of mana of any type: "any two mana".</summary>
/// <exampledoc>Advice from the Fae</exampledoc>
/// <examplecapture>any two mana</examplecapture>
[Dependent]
public class AnyAmountOfMana : Glyph
{
    public override Nib[] Nibs => ["any", Prop(Amount), "mana"];

    public Quantity Amount { get; set; }
}
