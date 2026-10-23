namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Burning Curiosity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Burning Curiosity</exampledoc>
[TokenizationOrder(-1)]
public class BurningCuriosityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this spell's additional cost was paid, exile the top three cards instead"];
}
