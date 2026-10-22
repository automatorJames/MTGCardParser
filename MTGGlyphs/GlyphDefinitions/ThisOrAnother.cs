namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A trigger subject naming the card itself and others like it: "this creature or another ally you control", "this creature or another creature or artifact you control", "{this} or another creature you control with flying", "this creature or another nontoken creature you control", "arahbo or another nontoken cat you control". The other permanent's kind is a referent, for a later "that creature".</summary>
/// <exampledoc>Zulaport Cutthroat</exampledoc>
/// <examplecapture>this creature or another creature you control</examplecapture>
[Dependent]
public class ThisOrAnother : Glyph
{
    public override Nib[] Nibs => [Prop(Self), "or another", Prop(Qualifier), Prop(Kind), Prop(Controller), Prop(With)];

    public OneOf<ThisCreature, This, ShortName> Self { get; set; }
    [Optional]
    public QualifierWords Qualifier { get; set; }
    [Referent]
    [Singular]
    public OneOf<ManyOf<PermanentKind>, PermanentKind> Kind { get; set; }
    [Optional]
    public ControlledBy Controller { get; set; }
    [Optional]
    public WithQualifier With { get; set; }
}
