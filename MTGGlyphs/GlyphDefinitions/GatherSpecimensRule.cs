namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Gather Specimens, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Gather Specimens</exampledoc>
[TokenizationOrder(-1)]
public class GatherSpecimensRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a creature would enter the battlefield under an opponent's control this turn, it enters under your control instead"];
}
