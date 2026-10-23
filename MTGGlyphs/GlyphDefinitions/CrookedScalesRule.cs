namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Crooked Scales, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Crooked Scales</exampledoc>
[TokenizationOrder(-1)]
public class CrookedScalesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you lose the flip, destroy target creature you control unless you pay {3} and repeat this process"];
}
