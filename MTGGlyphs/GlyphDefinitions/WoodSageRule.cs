namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Wood Sage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wood Sage</exampledoc>
[TokenizationOrder(-1)]
public class WoodSageRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["reveal the top four cards of your library and put all of them with that name into your hand"];
}
