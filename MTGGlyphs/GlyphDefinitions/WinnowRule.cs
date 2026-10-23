namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Winnow, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Winnow</exampledoc>
[TokenizationOrder(-1)]
public class WinnowRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy target nonland permanent if another permanent with the same name is on the battlefield"];
}
