namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Deep Freeze, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Deep Freeze</exampledoc>
/// <examplecapture>enchanted creature has base power and toughness 0/4, has defender, loses all other abilities, and is a blue wall in addition to its other colors and types</examplecapture>
[TokenizationOrder(-1)]
public class DeepFreezeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted creature has base power and toughness 0/4, has defender, loses all other abilities, and is a blue wall in addition to its other colors and types"];
}
