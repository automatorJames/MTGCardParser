namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Evolving Door, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Evolving Door</exampledoc>
/// <examplecapture>count the colors of the sacrificed creature, then search your library for a creature card that's exactly that many colors plus one</examplecapture>
[Dependent]
public class EvolvingDoorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["count the colors of the sacrificed creature, then search your library for a creature card that's exactly that many colors plus one"];
}
