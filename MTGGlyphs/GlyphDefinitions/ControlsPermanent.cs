namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player controlling, or not controlling, a particular permanent, one of a kind, or none of a kind: "you control {this}", "you control a snow mountain", "defending player controls a snow land", "you control no untapped lands", "you don't control an ogre".</summary>
/// <exampledoc>Aladdin</exampledoc>
/// <examplecapture>you control {this}</examplecapture>
[Dependent]
public class ControlsPermanent : Glyph
{
    public override Nib[] Nibs => [Prop(Controller), Prop(DoesNot), Pattern("controls?"), Prop(Permanent)];

    public WhichPlayer Controller { get; set; }
    [RegexPattern("(don't|doesn't)")]
    public bool DoesNot { get; set; }
    public OneOf<PermanentPhrase, IndefinitePermanent, NoPermanents> Permanent { get; set; }
}
