namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Multani, Maro-Sorcerer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Multani, Maro-Sorcerer</exampledoc>
[TokenizationOrder(-1)]
public class MultaniMaroSorcererRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["multani's power and toughness are each equal to the total number of cards in all players' hands"];
}
