namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player controlling a particular permanent, one of a kind, or none of a kind: "you control {this}", "you control a snow mountain", "defending player controls a snow land", "you control no untapped lands".</summary>
/// <exampledoc>Aladdin</exampledoc>
/// <examplecapture>you control {this}</examplecapture>
[Dependent]
public class ControlsPermanent : Glyph
{
    public override Nib[] Nibs => [Prop(Controller), Pattern("controls?"), Prop(Permanent)];

    public WhichPlayer Controller { get; set; }
    public OneOf<PermanentPhrase, IndefinitePermanent, NoPermanents> Permanent { get; set; }
}
