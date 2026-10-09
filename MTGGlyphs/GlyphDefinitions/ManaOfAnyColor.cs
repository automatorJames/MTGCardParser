namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"one mana of any color", "three mana of any one color": mana whose color the player chooses (all the same color, with "one").</summary>
/// <exampledoc>Birds of Paradise</exampledoc>
/// <examplecapture>one mana of any color</examplecapture>
[Dependent]
public class ManaOfAnyColor : Glyph
{
    public override Nib[] Nibs => [Prop(Quantity), "mana of any", Prop(SameColor), "color"];

    public Quantity Quantity { get; set; }
    [RegexPattern("one")]
    public bool SameColor { get; set; }
}
