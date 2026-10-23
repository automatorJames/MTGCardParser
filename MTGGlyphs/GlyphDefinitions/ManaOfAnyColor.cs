namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"one mana of any color", "three mana of any one color", "one mana of the chosen color", "one mana of any type that land produced", "one mana of any color that a land you control could produce", "one mana of any color among legendary permanents you control": mana whose color the player (or an earlier choice) picks.</summary>
/// <exampledoc>Birds of Paradise</exampledoc>
/// <examplecapture>one mana of any color</examplecapture>
[Dependent]
public class ManaOfAnyColor : Glyph
{
    public override Nib[] Nibs => [Prop(Quantity), "mana of", Prop(Choice)];

    public Quantity Quantity { get; set; }
    public OneOf<ManaChoice?, ManaProducibleBy, ColorAmongPermanents> Choice { get; set; }
}
