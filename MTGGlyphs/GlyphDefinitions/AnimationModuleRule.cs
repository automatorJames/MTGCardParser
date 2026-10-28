namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Animation Module, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Animation Module</exampledoc>
[TokenizationOrder(-1)]
public class AnimationModuleRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["give that permanent or player another counter of that kind"];
}
