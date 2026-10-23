namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player controlling, or not controlling, a particular permanent, one of a kind, or none of a kind, now or earlier: "you control {this}", "you control a snow mountain", "defending player controls a snow land", "you control no untapped lands", "you don't control an ogre", "you controlled that permanent", "that player controls a plains".</summary>
/// <exampledoc>Aladdin</exampledoc>
/// <examplecapture>you control {this}</examplecapture>
[Dependent]
public class ControlsPermanent : Glyph
{
    public override Nib[] Nibs => [Prop(Controller), Prop(DoesNot), Prop(Tense), Prop(Permanent)];

    public OneOf<WhichPlayer?, ThatPlayer> Controller { get; set; }
    [RegexPattern("(don't|doesn't)")]
    public bool DoesNot { get; set; }
    public ControlTense Tense { get; set; }
    public OneOf<PermanentPhrase, IndefinitePermanent, NoPermanents> Permanent { get; set; }
}
