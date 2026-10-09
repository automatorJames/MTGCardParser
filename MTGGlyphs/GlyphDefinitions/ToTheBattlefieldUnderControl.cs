namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Where a card is put: "to the battlefield under your control", "onto the battlefield under its owner's control".</summary>
/// <exampledoc>Animate Dead</exampledoc>
/// <examplecapture>to the battlefield under your control</examplecapture>
[Dependent]
public class ToTheBattlefieldUnderControl : Glyph
{
    public override Nib[] Nibs => [Alt("onto", "to"), "the battlefield under", Prop(Whose), "control"];

    public Whose Whose { get; set; }
}
