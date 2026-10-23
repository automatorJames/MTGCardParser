namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Swat Away, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Swat Away</exampledoc>
[TokenizationOrder(-1)]
public class SwatAwayRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this spell costs {2} less to cast if a creature is attacking you"];
}
