namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Obosh, the Preypiercer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Obosh, the Preypiercer</exampledoc>
[TokenizationOrder(-1)]
public class OboshThePreypiercer2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["companion — your starting deck contains only cards with odd mana values and land cards"];
}
