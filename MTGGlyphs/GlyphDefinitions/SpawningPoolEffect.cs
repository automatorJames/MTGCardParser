namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Spawning Pool, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spawning Pool</exampledoc>
[Dependent]
public class SpawningPoolEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this land becomes a 1/1 black skeleton creature with \"{b}: regenerate this creature\" until end of turn"];
}
