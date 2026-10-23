namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Divine Intervention, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Divine Intervention</exampledoc>
[TokenizationOrder(-1)]
public class DivineInterventionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when you remove the last intervention counter from this enchantment, the game is a draw"];
}
