namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"one mana of any color", "three mana of any one color", "one mana of the chosen color": mana whose color the player (or an earlier choice) picks.</summary>
/// <exampledoc>Birds of Paradise</exampledoc>
/// <examplecapture>one mana of any color</examplecapture>
[Dependent]
public class ManaOfAnyColor : Glyph
{
    public override Nib[] Nibs => [Prop(Quantity), "mana of", Prop(Choice)];

    public Quantity Quantity { get; set; }
    public ManaChoice Choice { get; set; }
}
