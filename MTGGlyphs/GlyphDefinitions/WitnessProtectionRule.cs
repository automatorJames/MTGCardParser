namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Witness Protection, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Witness Protection</exampledoc>
[TokenizationOrder(-1)]
public class WitnessProtectionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted creature loses all abilities and is a green and white citizen creature with base power and toughness 1/1 named legitimate businessperson"];
}
