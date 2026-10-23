namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Where a card is put: "to the battlefield under your control", "onto the battlefield tapped under its owner's control", "onto the battlefield tapped and attacking", "to the battlefield transformed under your control", "to the battlefield tapped and transformed under its owner's control", "to the battlefield under your control tapped and attacking", or just "to the battlefield" - optionally attached to something ("to the battlefield attached to that creature", for auras, equipment and curses) or with counters it enters with, "to the battlefield with a finality counter on it".</summary>
/// <exampledoc>Animate Dead</exampledoc>
/// <examplecapture>to the battlefield under your control</examplecapture>
[Dependent]
public class ToTheBattlefieldUnderControl : Glyph
{
    public override Nib[] Nibs => [Alt("onto", "to"), "the battlefield", Prop(Transformed), Prop(State), Prop(AndTransformed), Prop(Control), Prop(StateAfterControl), Prop(Attached), Prop(WithCounters)];

    [Optional]
    [RegexPattern("transformed")]
    public bool? Transformed { get; set; }
    [Optional]
    public EntryState? State { get; set; }
    [Optional]
    [RegexPattern("and transformed")]
    public bool? AndTransformed { get; set; }
    [Optional]
    public UnderControlOf Control { get; set; }
    [Optional]
    public EntryState? StateAfterControl { get; set; }
    [Optional]
    public AttachedToHost Attached { get; set; }
    [Optional]
    public EntersWithCounters WithCounters { get; set; }
}
