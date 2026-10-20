namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Several targets of one kind or a choice of kinds: "x target creatures", "two target artifacts", "up to one target artifact or enchantment", "up to two target creatures that player controls", "one or more target creatures", "x target creatures with power 2 or less". What they target is a referent, for a later "those creatures" or "they".</summary>
/// <exampledoc>Word of Binding</exampledoc>
/// <examplecapture>x target creatures</examplecapture>
[Dependent]
public class CountedTargets : Glyph
{
    public override Nib[] Nibs => [Prop(UpTo), Prop(Count), Prop(OrMore), "target", Prop(Qualifiers), Plural(Prop(Kind)), Prop(Controller), Prop(With)];

    [RegexPattern("up to")]
    public bool UpTo { get; set; }
    public Quantity Count { get; set; }
    [RegexPattern("or more")]
    public bool OrMore { get; set; }
    [Optional]
    [JoinedBy(Joiner.Space)]
    public CompoundOf<TargetQualifier> Qualifiers { get; set; }
    [Referent]
    [Plural]
    public OneOf<ManyOf<TargetKind>, TargetKind> Kind { get; set; }
    [Optional]
    public ControlledBy Controller { get; set; }
    [Optional]
    public WithQualifier With { get; set; }
}
