namespace MTGGlyphs.GlyphDefinitions;

/// <summary>You together with a group of your permanents, as one subject: "you and permanents you control", "you and other heroes you control" (hexproof grants), and in their reminder, "you and they".</summary>
/// <exampledoc>Lazotep Plating</exampledoc>
/// <examplecapture>you and permanents you control</examplecapture>
[Dependent]
public class YouAndPermanents : Glyph
{
    public override Nib[] Nibs => ["you and", Prop(Permanents)];

    public OneOf<OtherPermanents, Permanents, They> Permanents { get; set; }
}
