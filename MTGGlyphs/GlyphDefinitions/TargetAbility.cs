namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A targeted ability on the stack: "target activated ability", "target activated or triggered ability", "target spell or ability", possibly by who controls it ("target triggered ability you control", "… you don't control") and what its source is ("… from an artifact source").</summary>
/// <exampledoc>Stifle</exampledoc>
/// <examplecapture>target activated or triggered ability</examplecapture>
[Dependent]
public class TargetAbility : Glyph
{
    public override Nib[] Nibs => ["target", Prop(Kind), Prop(Controller), Prop(Source)];

    public StackAbilityKind Kind { get; set; }
    [Optional]
    public ControlledBy Controller { get; set; }
    [Optional]
    public FromSourceOfType Source { get; set; }
}
