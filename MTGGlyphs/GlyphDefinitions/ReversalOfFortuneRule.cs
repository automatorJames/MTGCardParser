namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Reversal of Fortune, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Reversal of Fortune</exampledoc>
[TokenizationOrder(-1)]
public class ReversalOfFortuneRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may copy an instant or sorcery card in it"];
}
