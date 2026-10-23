namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Triskaidekaphobia, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Triskaidekaphobia</exampledoc>
[TokenizationOrder(-1)]
public class TriskaidekaphobiaRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• each player with exactly 13 life loses the game, then each player gains 1 life"];
}
