namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Urza's Bauble, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Urza's Bauble</exampledoc>
[Dependent]
public class UrzasBaubleEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["look at a card at random in target player's hand"];
}
