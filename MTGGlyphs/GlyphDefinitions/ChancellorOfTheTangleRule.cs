namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Chancellor of the Tangle, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Chancellor of the Tangle</exampledoc>
[TokenizationOrder(-1)]
public class ChancellorOfTheTangleRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you do, at the beginning of your first main phase of the game, add {g}"];
}
