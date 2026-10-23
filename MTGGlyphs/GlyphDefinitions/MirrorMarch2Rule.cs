namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mirror March, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mirror March</exampledoc>
[TokenizationOrder(-1)]
public class MirrorMarch2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each flip you won, create a token that's a copy of that creature"];
}
