namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Phyrexian Grimoire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Phyrexian Grimoire</exampledoc>
[TokenizationOrder(-1)]
public class PhyrexianGrimoireCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile that card and put the other one into your hand"];
}
