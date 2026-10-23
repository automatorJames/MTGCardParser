namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Void Winnower, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Void Winnower</exampledoc>
[TokenizationOrder(-1)]
public class VoidWinnower2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["your opponents can't cast spells with even mana values"];
}
