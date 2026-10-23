namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Draugr Necromancer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Draugr Necromancer</exampledoc>
/// <examplecapture>you may cast spells from among cards in exile your opponents own with ice counters on them, and you may spend mana from snow sources as though it were mana of any color to cast those spells</examplecapture>
[TokenizationOrder(-1)]
public class DraugrNecromancerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may cast spells from among cards in exile your opponents own with ice counters on them, and you may spend mana from snow sources as though it were mana of any color to cast those spells"];
}
