namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ketramose, the New Dawn, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ketramose, the New Dawn</exampledoc>
[TokenizationOrder(-1)]
public class KetramoseTheNewDawnRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["ketramose can't attack or block unless there are seven or more cards in exile"];
}
