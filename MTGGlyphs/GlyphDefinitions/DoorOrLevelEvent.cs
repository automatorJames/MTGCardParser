namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The trigger events of rooms and classes: unlocking this door, or this class reaching a level.</summary>
/// <exampledoc>Wizard Class</exampledoc>
/// <examplecapture>this class becomes level 2</examplecapture>
[Dependent]
public class DoorOrLevelEvent : GlyphOneOf
{
    public UnlockThisDoor Unlock { get; set; }
    public ClassBecomesLevel Level { get; set; }
}
