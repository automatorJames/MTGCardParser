namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Fight with Fire, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fight with Fire</exampledoc>
[Dependent]
public class FightWithFireEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["those targets can include players and planeswalkers"];
}
