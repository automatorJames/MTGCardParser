namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"a land", "an artifact", "a non-wall creature", "a green or white creature", "a mountain an opponent controls", "an untapped creature with power 3 or greater": any one permanent of a kind, optionally narrowed and by who controls it. Its kind is a referent, for a later "that land".</summary>
/// <exampledoc>Ankh of Mishra</exampledoc>
/// <examplecapture>a land</examplecapture>
[Dependent]
public class IndefinitePermanent : Glyph
{
    public override Nib[] Nibs => [Pattern("an?"), Prop(Qualifier), Prop(Kind), Prop(Controller), Prop(With)];

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
