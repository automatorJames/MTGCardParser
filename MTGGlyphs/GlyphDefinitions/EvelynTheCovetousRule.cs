namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Evelyn, the Covetous, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Evelyn, the Covetous</exampledoc>
/// <examplecapture>once each turn, you may play a card from exile with a collection counter on it if it was exiled by an ability you controlled, and you may spend mana as though it were mana of any color to cast it</examplecapture>
[TokenizationOrder(-1)]
public class EvelynTheCovetousRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["once each turn, you may play a card from exile with a collection counter on it if it was exiled by an ability you controlled, and you may spend mana as though it were mana of any color to cast it"];
}
