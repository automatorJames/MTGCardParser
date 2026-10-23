namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Season of the Witch, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Season of the Witch</exampledoc>
[Dependent]
public class SeasonOfTheWitchEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all untapped creatures that didn't attack this turn, except for creatures that couldn't attack"];
}
