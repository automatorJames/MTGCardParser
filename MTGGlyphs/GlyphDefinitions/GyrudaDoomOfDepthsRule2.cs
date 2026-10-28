namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Gyruda, Doom of Depths, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gyruda, Doom of Depths</exampledoc>
[TokenizationOrder(-1)]
public class GyrudaDoomOfDepthsRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a creature card with an even mana value from among the milled cards onto the battlefield under your control"];
}
