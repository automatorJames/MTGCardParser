namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Where a card is put: "to the battlefield under your control", optionally "and attach {this} to it".</summary>
/// <exampledoc>Animate Dead</exampledoc>
/// <examplecapture>to the battlefield under your control and attach {this} to it</examplecapture>
public class ToTheBattlefieldUnderControl : Glyph
{
    public override Nib[] Nibs => [Alt("onto", "to"), "the battlefield under", Prop(Whose), "control", Prop(AndAttachThisToIt)];

    public Whose Whose { get; set; }

    [Optional]
    public AttachThisToIt AndAttachThisToIt { get; set; }
}