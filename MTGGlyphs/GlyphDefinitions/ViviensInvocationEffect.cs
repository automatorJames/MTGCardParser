namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Vivien's Invocation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Vivien's Invocation</exampledoc>
[Dependent]
public class ViviensInvocationEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it deals damage equal to its power to target creature an opponent controls"];
}
