namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sphinx of the Chimes, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sphinx of the Chimes</exampledoc>
[TokenizationOrder(-1)]
public class SphinxOfTheChimesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["discard two nonland cards with the same name: draw four cards"];
}
