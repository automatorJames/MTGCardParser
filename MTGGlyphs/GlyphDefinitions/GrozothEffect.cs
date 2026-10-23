namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Grozoth, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Grozoth</exampledoc>
/// <examplecapture>you may search your library for any number of cards that have mana value 9, reveal them, put them into your hand, then shuffle</examplecapture>
[Dependent]
public class GrozothEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may search your library for any number of cards that have mana value 9, reveal them, put them into your hand, then shuffle"];
}
