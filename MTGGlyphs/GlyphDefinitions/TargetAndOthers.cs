namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A target together with a group related to it: "target artifact and all other artifacts with the same name as that artifact", "target creature and each other creature with the same name as that creature" (the Echoing cycle, Maelstrom Pulse).</summary>
/// <exampledoc>Echoing Ruin</exampledoc>
/// <examplecapture>target artifact and all other artifacts with the same name as that artifact</examplecapture>
[Dependent]
public class TargetAndOthers : Glyph
{
    public override Nib[] Nibs => [Prop(Target), "and", Prop(Others)];

    public SpecificTarget Target { get; set; }
    public OneOf<PermanentGroup, EachPermanents> Others { get; set; }
}
