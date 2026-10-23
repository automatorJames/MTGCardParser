namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Spawning Pool, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spawning Pool</exampledoc>
[Dependent]
public class SpawningPoolEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it regenerates, the next time it would be destroyed this turn, instead tap it, remove it from combat, and heal all damage on it"];
}
