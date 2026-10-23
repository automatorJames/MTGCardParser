namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Fathom Trawl, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fathom Trawl</exampledoc>
/// <examplecapture>put the nonland cards revealed this way into your hand, then put the rest of the revealed cards on the bottom of your library in any order</examplecapture>
[TokenizationOrder(-1)]
public class FathomTrawlRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put the nonland cards revealed this way into your hand, then put the rest of the revealed cards on the bottom of your library in any order"];
}
