namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Kingpin of Crime, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Kingpin of Crime</exampledoc>
/// <examplecapture>if you do, until end of turn, creatures you control with toughness greater than their power assign combat damage equal to their toughness rather than their power</examplecapture>
[TokenizationOrder(-1)]
public class TheKingpinOfCrimeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do, until end of turn, creatures you control with toughness greater than their power assign combat damage equal to their toughness rather than their power"];
}
