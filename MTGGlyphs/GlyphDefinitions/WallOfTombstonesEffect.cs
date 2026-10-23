namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Wall of Tombstones, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wall of Tombstones</exampledoc>
[Dependent]
public class WallOfTombstonesEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature's base toughness becomes equal to 1 plus the number of creature cards in your graveyard"];
}
