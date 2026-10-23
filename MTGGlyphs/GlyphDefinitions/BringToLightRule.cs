namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Bring to Light, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bring to Light</exampledoc>
/// <examplecapture>converge — search your library for a creature, instant, or sorcery card with mana value less than or equal to the number of colors of mana spent to cast this spell, exile that card, then shuffle</examplecapture>
[TokenizationOrder(-1)]
public class BringToLightRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["converge — search your library for a creature, instant, or sorcery card with mana value less than or equal to the number of colors of mana spent to cast this spell, exile that card, then shuffle"];
}
