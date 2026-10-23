namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Lithoform Blight, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lithoform Blight</exampledoc>
[TokenizationOrder(-1)]
public class LithoformBlightRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["enchanted land loses all land types and abilities and has \"{t}: add {c}\" and \"{t}, pay 1 life: add one mana of any color"];
}
