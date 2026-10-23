namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Keeper of the Dead, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Keeper of the Dead</exampledoc>
[Dependent]
public class KeeperOfTheDeadEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose target opponent who has at least two fewer creature cards in their graveyard than you do as you activate this ability"];
}
