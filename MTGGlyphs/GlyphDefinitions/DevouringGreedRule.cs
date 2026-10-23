namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Devouring Greed, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Devouring Greed</exampledoc>
[TokenizationOrder(-1)]
public class DevouringGreedRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target player loses 2 life plus 2 life for each spirit sacrificed this way"];
}
