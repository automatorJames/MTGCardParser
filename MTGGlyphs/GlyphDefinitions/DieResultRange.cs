namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The results a die-table row covers: "1—9", or a single result "20".</summary>
/// <exampledoc>Goblin Morningstar</exampledoc>
/// <examplecapture>1—9</examplecapture>
[Dependent]
public class DieResultRange : Glyph
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => [Prop(Low), Prop(High)];

    public int Low { get; set; }
    [Optional]
    public DieResultUpper High { get; set; }
}
