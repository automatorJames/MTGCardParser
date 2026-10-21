namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A targeted ability on the stack: "target activated ability", "target activated or triggered ability", "target spell or ability".</summary>
/// <exampledoc>Stifle</exampledoc>
/// <examplecapture>target activated or triggered ability</examplecapture>
[Dependent]
public class TargetAbility : Glyph
{
    public override Nib[] Nibs => ["target", Prop(Kind)];

    public StackAbilityKind Kind { get; set; }
}
