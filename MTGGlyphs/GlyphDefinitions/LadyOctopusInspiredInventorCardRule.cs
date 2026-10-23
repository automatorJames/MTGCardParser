namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Lady Octopus, Inspired Inventor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lady Octopus, Inspired Inventor</exampledoc>
[TokenizationOrder(-1)]
public class LadyOctopusInspiredInventorCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you draw your first or second card each turn, put an ingenuity counter on lady octopus"];
}
