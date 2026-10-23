namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Juxtapose, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Juxtapose</exampledoc>
[TokenizationOrder(-1)]
public class JuxtaposeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you and target player exchange control of the creature you each control with the greatest mana value"];
}
