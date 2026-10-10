namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Where a card is put: "to the battlefield under your control", "onto the battlefield under its owner's control", or just "to the battlefield".</summary>
/// <exampledoc>Animate Dead</exampledoc>
/// <examplecapture>to the battlefield under your control</examplecapture>
[Dependent]
public class ToTheBattlefieldUnderControl : Glyph
{
    public override Nib[] Nibs => [Alt("onto", "to"), "the battlefield", Prop(Control)];

    [Optional]
    public UnderControlOf Control { get; set; }
}
