namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Goblin Welder, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Goblin Welder</exampledoc>
[Dependent]
public class GoblinWelderEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose target artifact a player controls and target artifact card in that player's graveyard"];
}
