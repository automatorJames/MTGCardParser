namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"a land", "an artifact", "a non-wall creature", "a green or white creature", "a mountain an opponent controls", "an untapped creature with power 3 or greater", "another creature you control": any one permanent of a kind, optionally other than this one, narrowed and by who controls it. Its kind is a referent, for a later "that land".</summary>
/// <exampledoc>Ankh of Mishra</exampledoc>
/// <examplecapture>a land</examplecapture>
[Dependent]
public class IndefinitePermanent : Glyph
{
    public override Nib[] Nibs => [Prop(Determiner), Prop(Qualifier), Prop(Kind), Prop(Controller), Prop(With)];

    public IndefiniteDeterminer Determiner { get; set; }
    [Optional]
    public OneOf<ManyOf<TargetQualifier>, TargetQualifier> Qualifier { get; set; }
    [Referent]
    [Singular]
    public PermanentKind Kind { get; set; }
    [Optional]
    public ControlledBy Controller { get; set; }
    [Optional]
    public WithQualifier With { get; set; }
}
