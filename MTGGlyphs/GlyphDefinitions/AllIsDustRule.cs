namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of All Is Dust, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>All Is Dust</exampledoc>
[TokenizationOrder(-1)]
public class AllIsDustRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player sacrifices all permanents they control that are one or more colors"];
}
