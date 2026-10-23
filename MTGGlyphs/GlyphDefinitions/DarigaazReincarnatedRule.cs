namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Darigaaz Reincarnated, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Darigaaz Reincarnated</exampledoc>
[TokenizationOrder(-1)]
public class DarigaazReincarnatedRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if darigaaz would die, instead exile it with three egg counters on it"];
}
