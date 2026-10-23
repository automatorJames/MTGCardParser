namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tinybones, Bauble Burglar, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tinybones, Bauble Burglar</exampledoc>
/// <examplecapture>during your turn, you may play cards you don't own with stash counters on them from exile, and mana of any type can be spent to cast those spells</examplecapture>
[TokenizationOrder(-1)]
public class TinybonesBaubleBurglarRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["during your turn, you may play cards you don't own with stash counters on them from exile, and mana of any type can be spent to cast those spells"];
}
