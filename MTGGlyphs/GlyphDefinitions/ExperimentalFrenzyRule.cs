namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Experimental Frenzy, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Experimental Frenzy</exampledoc>
[TokenizationOrder(-1)]
public class ExperimentalFrenzyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you can't play lands or cast spells from your hand"];
}
