namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Alpine Houndmaster, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Alpine Houndmaster</exampledoc>
/// <examplecapture>you may search your library for a card named alpine watchdog and/or a card named igneous cur, reveal them, put them into your hand, then shuffle</examplecapture>
[Dependent]
public class AlpineHoundmasterEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may search your library for a card named alpine watchdog and/or a card named igneous cur, reveal them, put them into your hand, then shuffle"];
}
