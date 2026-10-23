namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Moonsilver Key, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Moonsilver Key</exampledoc>
/// <examplecapture>search your library for an artifact card with a mana ability or a basic land card, reveal it, put it into your hand, then shuffle</examplecapture>
[Dependent]
public class MoonsilverKeyEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["search your library for an artifact card with a mana ability or a basic land card, reveal it, put it into your hand, then shuffle"];
}
