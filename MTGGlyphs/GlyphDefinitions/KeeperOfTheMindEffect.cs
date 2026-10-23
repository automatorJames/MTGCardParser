namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Keeper of the Mind, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Keeper of the Mind</exampledoc>
[Dependent]
public class KeeperOfTheMindEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose target opponent who has at least two more cards in hand than you do as you activate this ability"];
}
