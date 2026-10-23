namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Sphinx of the Chimes, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sphinx of the Chimes</exampledoc>
[Dependent]
public class SphinxOfTheChimesEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw four cards"];
}
