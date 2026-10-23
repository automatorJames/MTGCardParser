namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Aurora Awakener, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Aurora Awakener</exampledoc>
/// <examplecapture>put any number of those permanent cards onto the battlefield, then put the rest of the revealed cards on the bottom of your library in a random order</examplecapture>
[TokenizationOrder(-1)]
public class AuroraAwakenerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put any number of those permanent cards onto the battlefield, then put the rest of the revealed cards on the bottom of your library in a random order"];
}
