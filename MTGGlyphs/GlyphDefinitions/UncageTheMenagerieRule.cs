namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Uncage the Menagerie, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Uncage the Menagerie</exampledoc>
/// <examplecapture>search your library for up to x creature cards with different names that each have mana value x, reveal them, put them into your hand, then shuffle</examplecapture>
[TokenizationOrder(-1)]
public class UncageTheMenagerieRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["search your library for up to x creature cards with different names that each have mana value x, reveal them, put them into your hand, then shuffle"];
}
