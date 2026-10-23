namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mindstorm Crown, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mindstorm Crown</exampledoc>
[TokenizationOrder(-1)]
public class MindstormCrown2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you had a card in hand, this artifact deals 1 damage to you"];
}
