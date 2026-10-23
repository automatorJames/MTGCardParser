namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Several targets of one kind or a choice of kinds: "x target creatures", "two target artifacts", "up to one target artifact or enchantment", "up to two target creatures that player controls", "one or more target creatures", "up to two other target creatures", "x target creatures with power 2 or less", "two target artifacts and/or enchantments", "two target creatures controlled by the same player", "two target permanents that share a card type". What they target is a referent, for a later "those creatures" or "they". "or more" and "other" share one optional enum, since two optional properties in a row don't match.</summary>
/// <exampledoc>Word of Binding</exampledoc>
/// <examplecapture>x target creatures</examplecapture>
[Dependent]
public class CountedTargets : Glyph
{
    public override Nib[] Nibs => [Prop(UpTo), Prop(Count), Prop(Suffix), "target", Prop(Qualifiers), Prop(Kind), Prop(Controller), Prop(With), Prop(Sharing)];

    [RegexPattern("up to")]
    public bool UpTo { get; set; }
    public Quantity Count { get; set; }
    [Optional]
    public CountSuffix? Suffix { get; set; }
    [Optional]
    [JoinedBy(Joiner.Space)]
    public CompoundOf<TargetQualifier> Qualifiers { get; set; }
    [Referent]
    [Plural]
    public OneOf<PluralKindsAndOr, ManyOf<PluralTargetKind>, PluralTargetKind> Kind { get; set; }
    [Optional]
    public OneOf<ControlledBy, ControlledByShared> Controller { get; set; }
    [Optional]
    public WithQualifier With { get; set; }
    [Optional]
    public ShareAnAspect Sharing { get; set; }
}
