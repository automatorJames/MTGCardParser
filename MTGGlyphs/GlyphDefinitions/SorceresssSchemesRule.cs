namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sorceress's Schemes, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sorceress's Schemes</exampledoc>
[TokenizationOrder(-1)]
public class SorceresssSchemesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return target instant or sorcery card from your graveyard or exiled card with flashback you own to your hand"];
}
