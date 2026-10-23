namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Animation Module, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Animation Module</exampledoc>
[Dependent]
public class AnimationModuleEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose a counter on target permanent or player"];
}
