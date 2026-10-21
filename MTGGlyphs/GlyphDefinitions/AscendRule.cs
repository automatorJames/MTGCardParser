namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Reminder text for a keyword that gives its controller a lasting designation once they control enough permanents: ascend ("if you control ten or more permanents, you get the city's blessing for the rest of the game") and storied ("… you have an enduring story …").</summary>
/// <exampledoc>Arch of Orazca</exampledoc>
/// <examplecapture>if you control ten or more permanents, you get the city's blessing for the rest of the game</examplecapture>
public class AscendRule : Glyph
{
    public override Nib[] Nibs => ["if you control", Prop(Threshold), ", you", Alt("get", "have"), Prop(Designation), "for the rest of the game"];

    public PermanentsThreshold Threshold { get; set; }
    public PlayerDesignation Designation { get; set; }
}
