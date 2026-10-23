namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Central Elevator // Promising Stairs, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Central Elevator // Promising Stairs</exampledoc>
/// <examplecapture>search your library for a room card that doesn't have the same name as a room you control, reveal it, put it into your hand, then shuffle</examplecapture>
[Dependent]
public class CentralElevatorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["search your library for a room card that doesn't have the same name as a room you control, reveal it, put it into your hand, then shuffle"];
}
