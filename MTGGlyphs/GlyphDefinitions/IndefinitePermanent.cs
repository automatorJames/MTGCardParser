namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"a land", "an artifact", "a non-wall creature", "a green or white creature", "a nontoken white permanent", "a nonland, nontoken permanent", "an artifact or creature", "a mountain an opponent controls", "an untapped creature with power 3 or greater", "another creature you control", "a creature other than this creature", "a permanent that's one or more colors", "a creature of their choice with flying", "a permanent you control of the chosen type", "another permanent you control that shares a permanent type with it": any one permanent of a kind (or of one of several kinds), optionally other than this one, chosen by a given player, narrowed (by a with-qualifier, a chosen aspect, or an aspect shared with another object) and by who controls it. Its kind is a referent, for a later "that land".</summary>
/// <exampledoc>Ankh of Mishra</exampledoc>
/// <examplecapture>a land</examplecapture>
[Dependent]
public class IndefinitePermanent : Glyph
{
    public override Nib[] Nibs => [Prop(Determiner), Prop(Qualifier), Prop(Kind), Prop(Controller), Prop(OtherThan), Prop(ChosenBy), Prop(With), Prop(Colored)];

    public IndefiniteDeterminer Determiner { get; set; }
    [Optional]
    public OneOf<ManyOf<TargetQualifier>, OneOf<TargetQualifier, CompoundOf<TargetQualifier>>, QualifierWords> Qualifier { get; set; }
    [Referent]
    [Singular]
    public OneOf<ManyOf<PermanentKind>, PermanentKind> Kind { get; set; }
    [Optional]
    public ControlledBy Controller { get; set; }
    [Optional]
    public OtherThanThis OtherThan { get; set; }
    [Optional]
    public OfPlayersChoice ChosenBy { get; set; }
    [Optional]
    public OneOf<WithQualifier, ChosenAspect?, SharesCreatureType> With { get; set; }
    [Optional]
    public ThatsColored Colored { get; set; }
}
