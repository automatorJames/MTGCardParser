namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"a land", "an artifact": any one permanent of a kind. Its kind is a referent, for a later "that land".</summary>
/// <exampledoc>Ankh of Mishra</exampledoc>
/// <examplecapture>a land</examplecapture>
[Dependent]
public class IndefinitePermanent : Glyph
{
    public override Nib[] Nibs => [Pattern("an?"), Prop(Kind)];

    [Referent]
    [Singular]
    public PermanentKind Kind { get; set; }
}
