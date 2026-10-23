namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Svella, Ice Shaper, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Svella, Ice Shaper</exampledoc>
[Dependent]
public class SvellaIceShaperEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a colorless snow artifact token named icy manalith with \"{t}: add one mana of any color"];
}
