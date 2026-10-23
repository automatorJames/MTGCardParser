namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Finale of Promise, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Finale of Promise</exampledoc>
/// <examplecapture>you may cast up to one target instant card and/or up to one target sorcery card from your graveyard each with mana value x or less without paying their mana costs</examplecapture>
[TokenizationOrder(-1)]
public class FinaleOfPromiseRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast up to one target instant card and/or up to one target sorcery card from your graveyard each with mana value x or less without paying their mana costs"];
}
