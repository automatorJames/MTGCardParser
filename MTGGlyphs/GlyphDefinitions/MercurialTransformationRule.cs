namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mercurial Transformation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mercurial Transformation</exampledoc>
/// <examplecapture>until end of turn, target nonland permanent loses all abilities and becomes your choice of a blue frog creature with base power and toughness 1/1 or a blue octopus creature with base power and toughness 4/4</examplecapture>
[TokenizationOrder(-1)]
public class MercurialTransformationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn, target nonland permanent loses all abilities and becomes your choice of a blue frog creature with base power and toughness 1/1 or a blue octopus creature with base power and toughness 4/4"];
}
