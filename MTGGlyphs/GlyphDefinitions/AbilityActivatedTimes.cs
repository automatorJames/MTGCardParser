namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How often this ability has been activated this turn, against a threshold: "this ability has been activated four or more times this turn".</summary>
/// <exampledoc>Dragon Whelp</exampledoc>
/// <examplecapture>this ability has been activated four or more times this turn</examplecapture>
[Dependent]
public class AbilityActivatedTimes : Glyph
{
    public override Nib[] Nibs => ["this ability has been activated", Prop(Count), "times this turn"];

    public AtLeastCount Count { get; set; }
}
