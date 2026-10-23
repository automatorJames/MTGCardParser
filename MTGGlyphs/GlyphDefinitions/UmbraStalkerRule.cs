namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Umbra Stalker, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Umbra Stalker</exampledoc>
[TokenizationOrder(-1)]
public class UmbraStalkerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["chroma —", Nib.This, "'s power and toughness are each equal to the number of black mana symbols in the mana costs of cards in your graveyard"];
}
