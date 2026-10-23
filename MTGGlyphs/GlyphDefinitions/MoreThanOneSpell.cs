namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"more than one spell each turn", "more than one noncreature spell each turn", "more than one spell this way each turn": a limit of one spell (of a kind) per turn.</summary>
/// <exampledoc>Colfenor's Plans</exampledoc>
/// <examplecapture>more than one spell each turn</examplecapture>
[Dependent]
public class MoreThanOneSpell : Glyph
{
    public override Nib[] Nibs => ["more than one", Prop(Kind), "spell", Opt("this way"), "each turn"];

    [Optional]
    public TargetQualifier Kind { get; set; }
}
