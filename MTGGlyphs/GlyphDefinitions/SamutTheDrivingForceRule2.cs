namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Samut, the Driving Force, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Samut, the Driving Force</exampledoc>
[TokenizationOrder(-1)]
public class SamutTheDrivingForceRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["noncreature spells you cast cost {x} less to cast, where x is your speed"];
}
