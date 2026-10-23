namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Legerdemain, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Legerdemain</exampledoc>
[TokenizationOrder(-1)]
public class LegerdemainRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exchange control of target artifact or creature and another target permanent that shares one of those types with it"];
}
