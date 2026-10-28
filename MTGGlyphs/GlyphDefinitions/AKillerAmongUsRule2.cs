namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of A Killer Among Us, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>A Killer Among Us</exampledoc>
[TokenizationOrder(-1)]
public class AKillerAmongUsRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["sacrifice this enchantment, reveal the creature type you chose: if target attacking creature token is the chosen type, put three +1/+1 counters on it and it gains deathtouch until end of turn"];
}
