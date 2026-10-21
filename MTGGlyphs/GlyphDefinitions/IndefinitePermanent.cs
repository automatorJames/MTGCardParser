namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"a land", "an artifact", "a non-wall creature", "a green or white creature", "a nontoken white permanent", "an artifact or creature", "a mountain an opponent controls", "an untapped creature with power 3 or greater", "another creature you control": any one permanent of a kind (or of one of several kinds), optionally other than this one, narrowed and by who controls it. Its kind is a referent, for a later "that land".</summary>
/// <exampledoc>Ankh of Mishra</exampledoc>
/// <examplecapture>a land</examplecapture>
[Dependent]
public class IndefinitePermanent : Glyph
{
    public override Nib[] Nibs => [Prop(Determiner), Prop(Qualifier), Prop(Kind), Prop(Controller), Prop(With)];

    public IndefiniteDeterminer Determiner { get; set; }
    [Optional]
    public OneOf<ManyOf<TargetQualifier>, TargetQualifier, QualifierWords> Qualifier { get; set; }
    [Referent]
    [Singular]
    public OneOf<ManyOf<PermanentKind>, PermanentKind> Kind { get; set; }
    [Optional]
    public ControlledBy Controller { get; set; }
    [Optional]
    public WithQualifier With { get; set; }
}
