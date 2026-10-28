namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Two-Headed Giant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Two-Headed Giant</exampledoc>
[TokenizationOrder(-1)]
public class TwoHeadedGiantRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if both coins come up tails, this creature gains menace until end of turn"];
}
