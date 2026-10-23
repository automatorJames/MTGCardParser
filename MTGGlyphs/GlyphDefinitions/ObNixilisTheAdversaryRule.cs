namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ob Nixilis, the Adversary, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ob Nixilis, the Adversary</exampledoc>
[TokenizationOrder(-1)]
public class ObNixilisTheAdversaryRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the copy isn't legendary and has starting loyalty x"];
}
