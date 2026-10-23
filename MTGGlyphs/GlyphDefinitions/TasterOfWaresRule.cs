namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Taster of Wares, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Taster of Wares</exampledoc>
/// <examplecapture>if an instant or sorcery card is exiled this way, you may cast it for as long as you control this creature, and mana of any type can be spent to cast that spell</examplecapture>
[TokenizationOrder(-1)]
public class TasterOfWaresRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if an instant or sorcery card is exiled this way, you may cast it for as long as you control this creature, and mana of any type can be spent to cast that spell"];
}
