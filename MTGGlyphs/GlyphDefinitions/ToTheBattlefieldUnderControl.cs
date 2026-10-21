namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Where a card is put: "to the battlefield under your control", "onto the battlefield tapped under its owner's control", "to the battlefield transformed under your control", or just "to the battlefield".</summary>
/// <exampledoc>Animate Dead</exampledoc>
/// <examplecapture>to the battlefield under your control</examplecapture>
[Dependent]
public class ToTheBattlefieldUnderControl : Glyph
{
    public override Nib[] Nibs => [Alt("onto", "to"), "the battlefield", Prop(Transformed), Prop(Tapped), Prop(Control)];

    [Optional]
    [RegexPattern("transformed")]
    public bool? Transformed { get; set; }
    [Optional]
    [RegexPattern("tapped")]
    public bool? Tapped { get; set; }
    [Optional]
    public UnderControlOf Control { get; set; }
}
