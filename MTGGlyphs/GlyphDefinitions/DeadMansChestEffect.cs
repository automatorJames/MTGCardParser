namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Dead Man's Chest, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dead Man's Chest</exampledoc>
[Dependent]
public class DeadMansChestEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile cards equal to its power from the top of its owner's library"];
}
