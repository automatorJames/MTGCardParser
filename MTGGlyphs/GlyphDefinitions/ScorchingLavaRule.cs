namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Scorching Lava, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Scorching Lava</exampledoc>
[TokenizationOrder(-1)]
public class ScorchingLavaRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this spell was kicked, that creature can't be regenerated this turn and if it would die this turn, exile it instead"];
}
