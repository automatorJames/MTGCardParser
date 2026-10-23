namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sideswipe, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sideswipe</exampledoc>
[TokenizationOrder(-1)]
public class SideswipeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may change any targets of target arcane spell"];
}
