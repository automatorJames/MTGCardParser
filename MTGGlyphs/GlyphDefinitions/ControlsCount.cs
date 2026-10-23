namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"controls more lands", "controls fewer creatures": a player's count of permanents of a kind, compared.</summary>
/// <exampledoc>Oath of Lieges</exampledoc>
/// <examplecapture>controls more lands</examplecapture>
[Dependent]
public class ControlsCount : Glyph
{
    public override Nib[] Nibs => [Pattern("controls?"), Prop(Bound), Prop(Permanents)];

    public CountBound Bound { get; set; }
    public Permanents Permanents { get; set; }
}
