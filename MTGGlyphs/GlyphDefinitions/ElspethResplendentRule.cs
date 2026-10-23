namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Elspeth Resplendent, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Elspeth Resplendent</exampledoc>
[TokenizationOrder(-1)]
public class ElspethResplendentRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter and a counter from among flying, first strike, lifelink, or vigilance on it"];
}
