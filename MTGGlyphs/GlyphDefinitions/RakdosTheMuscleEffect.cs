namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Rakdos, the Muscle, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rakdos, the Muscle</exampledoc>
[Dependent]
public class RakdosTheMuscleEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile cards equal to its mana value from the top of target player's library"];
}
