namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Gyruda, Doom of Depths, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gyruda, Doom of Depths</exampledoc>
[TokenizationOrder(-1)]
public class GyrudaDoomOfDepthsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["companion — your starting deck contains only cards with even mana values"];
}
