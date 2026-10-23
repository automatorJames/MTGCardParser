namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Pardic Firecat, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Pardic Firecat</exampledoc>
[TokenizationOrder(-1)]
public class PardicFirecatRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if this card is in a graveyard, effects from spells named flame burst count it as a card named flame burst"];
}
