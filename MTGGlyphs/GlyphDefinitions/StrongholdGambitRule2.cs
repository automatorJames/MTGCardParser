namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Stronghold Gambit, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Stronghold Gambit</exampledoc>
[TokenizationOrder(-1)]
public class StrongholdGambitRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the owner of each creature card revealed this way with the lowest mana value puts it onto the battlefield"];
}
