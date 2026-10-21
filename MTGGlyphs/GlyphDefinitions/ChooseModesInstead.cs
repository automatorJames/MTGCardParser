namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Choosing more modes than usual, as the outcome of a condition: "choose both instead", "you may choose both instead", "choose any number instead".</summary>
/// <exampledoc>Molten Collapse</exampledoc>
/// <examplecapture>you may choose both instead</examplecapture>
[Dependent]
public class ChooseModesInstead : Glyph
{
    public override Nib[] Nibs => [Prop(May), "choose", Prop(Count), "instead"];

    [RegexPattern("you may")]
    public bool May { get; set; }
    public ModeCount Count { get; set; }
}
