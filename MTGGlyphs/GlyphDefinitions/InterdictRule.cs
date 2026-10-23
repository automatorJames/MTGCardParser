namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Interdict, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Interdict</exampledoc>
[TokenizationOrder(-1)]
public class InterdictRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter target activated ability from an artifact, creature, enchantment, or land"];
}
