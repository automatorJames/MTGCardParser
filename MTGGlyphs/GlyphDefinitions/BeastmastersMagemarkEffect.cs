namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Beastmaster's Magemark, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Beastmaster's Magemark</exampledoc>
[Dependent]
public class BeastmastersMagemarkEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it gets +1/+1 until end of turn for each creature blocking it"];
}
