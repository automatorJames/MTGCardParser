namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Temporal Aperture, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Temporal Aperture</exampledoc>
/// <examplecapture>until end of turn, for as long as that card remains on top of your library, play with the top card of your library revealed and you may play that card without paying its mana cost</examplecapture>
[TokenizationOrder(-1)]
public class TemporalApertureRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn, for as long as that card remains on top of your library, play with the top card of your library revealed and you may play that card without paying its mana cost"];
}
