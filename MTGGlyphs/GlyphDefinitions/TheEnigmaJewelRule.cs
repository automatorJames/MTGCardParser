namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Enigma Jewel // Locus of Enlightenment, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Enigma Jewel // Locus of Enlightenment</exampledoc>
/// <examplecapture>craft with four or more nonlands with activated abilities {8}{u} ({8}{u}, exile this artifact, exile the four or more from among other permanents you control and/or cards in your graveyard: return this card transformed under its owner's control</examplecapture>
[TokenizationOrder(-1)]
public class TheEnigmaJewelRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["craft with four or more nonlands with activated abilities {8}{u} ({8}{u}, exile this artifact, exile the four or more from among other permanents you control and/or cards in your graveyard: return this card transformed under its owner's control"];
}
