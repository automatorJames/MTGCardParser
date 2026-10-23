namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Detection Tower, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Detection Tower</exampledoc>
/// <examplecapture>until end of turn, your opponents and creatures your opponents control with hexproof can be the targets of spells and abilities you control as though they didn't have hexproof</examplecapture>
[Dependent]
public class DetectionTowerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["until end of turn, your opponents and creatures your opponents control with hexproof can be the targets of spells and abilities you control as though they didn't have hexproof"];
}
