namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tempest Efreet, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tempest Efreet</exampledoc>
[TokenizationOrder(-1)]
public class TempestEfreetRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put the revealed card into your hand and", Nib.This, "from anywhere into that player's graveyard"];
}
