namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Cyclopean Tomb, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cyclopean Tomb</exampledoc>
/// <examplecapture>at the beginning of each of your upkeeps for the rest of the game, remove all mire counters from a land that a mire counter was put onto with this artifact but that a mire counter has not been removed from with this artifact</examplecapture>
[Dependent]
public class CyclopeanTombEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["at the beginning of each of your upkeeps for the rest of the game, remove all mire counters from a land that a mire counter was put onto with this artifact but that a mire counter has not been removed from with this artifact"];
}
