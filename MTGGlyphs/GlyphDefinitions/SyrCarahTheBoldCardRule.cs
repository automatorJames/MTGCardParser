namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Syr Carah, the Bold, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Syr Carah, the Bold</exampledoc>
[TokenizationOrder(-1)]
public class SyrCarahTheBoldCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever syr carah or an instant or sorcery spell you control deals damage to a player, exile the top card of your library"];
}
