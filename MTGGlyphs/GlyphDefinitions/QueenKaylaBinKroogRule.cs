namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Queen Kayla bin-Kroog, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Queen Kayla bin-Kroog</exampledoc>
/// <examplecapture>you may choose an artifact or creature card with mana value 1 you discarded this way, then do the same for artifact or creature cards with mana values 2 and 3</examplecapture>
[TokenizationOrder(-1)]
public class QueenKaylaBinKroogRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may choose an artifact or creature card with mana value 1 you discarded this way, then do the same for artifact or creature cards with mana values 2 and 3"];
}
