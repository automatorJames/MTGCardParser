namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Demonic Attorney, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Demonic Attorney</exampledoc>
[TokenizationOrder(-1)]
public class DemonicAttorneyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player antes the top card of their library"];
}
