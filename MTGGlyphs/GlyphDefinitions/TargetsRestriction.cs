namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Which spells or abilities a retargeting effect can pick: "with a single target", "that targets only this creature", "that targets only a single creature", "that targets only a player".</summary>
/// <exampledoc>Deflection</exampledoc>
/// <examplecapture>with a single target</examplecapture>
[Dependent]
public class TargetsRestriction : Glyph
{
    public override Nib[] Nibs => [Alt("with a single target", "that targets only"), Prop(OnlyTarget)];

    [Optional]
    public OneOf<ThisCreature, SingleTargetKind> OnlyTarget { get; set; }
}
