namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of The Phasing of Zhalfir, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>The Phasing of Zhalfir</exampledoc>
[TokenizationOrder(-1)]
public class ThePhasingOfZhalfirRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it can't phase in for as long as you control this saga"];
}
