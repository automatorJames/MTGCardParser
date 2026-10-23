namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Identity Crisis, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Identity Crisis</exampledoc>
[TokenizationOrder(-1)]
public class IdentityCrisisRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile all cards from target player's hand and graveyard"];
}
