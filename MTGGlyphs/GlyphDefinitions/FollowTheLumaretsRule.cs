namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Follow the Lumarets, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Follow the Lumarets</exampledoc>
[TokenizationOrder(-1)]
public class FollowTheLumaretsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you gained life this turn, you may instead reveal two creature and/or land cards from among them and put them into your hand"];
}
