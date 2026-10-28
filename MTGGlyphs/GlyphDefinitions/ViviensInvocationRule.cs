namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Vivien's Invocation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vivien's Invocation</exampledoc>
[TokenizationOrder(-1)]
public class ViviensInvocationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when a creature is put onto the battlefield this way, it deals damage equal to its power to target creature an opponent controls"];
}
