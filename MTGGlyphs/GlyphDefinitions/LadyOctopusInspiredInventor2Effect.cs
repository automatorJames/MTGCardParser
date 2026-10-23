namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Lady Octopus, Inspired Inventor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lady Octopus, Inspired Inventor</exampledoc>
[Dependent]
public class LadyOctopusInspiredInventor2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put an ingenuity counter on lady octopus"];
}
