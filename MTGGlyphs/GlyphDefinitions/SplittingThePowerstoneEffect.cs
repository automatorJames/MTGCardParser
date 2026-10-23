namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Splitting the Powerstone, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Splitting the Powerstone</exampledoc>
[Dependent]
public class SplittingThePowerstoneEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the tokens are artifacts with \"{t}: add {c}"];
}
