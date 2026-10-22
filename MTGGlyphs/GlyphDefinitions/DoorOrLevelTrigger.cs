namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A room's or class's triggered ability: "when you unlock this door, manifest dread", "when this class becomes level 2, draw two cards".</summary>
/// <exampledoc>Wizard Class</exampledoc>
/// <examplecapture>when this class becomes level 2, draw two cards</examplecapture>
public class DoorOrLevelTrigger : Glyph
{
    public override Nib[] Nibs => ["when", Prop(Event), ",", Prop(Effect)];

    public DoorOrLevelEvent Event { get; set; }
    public DynamicGlyph Effect { get; set; }
}
