namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"to {this}", "to any target": what a prevention effect protects.</summary>
/// <exampledoc>Samite Healer</exampledoc>
/// <examplecapture>to any target</examplecapture>
[Dependent]
public class ProtectedRecipient : Glyph
{
    public override Nib[] Nibs => ["to", Prop(Who)];

    public OneOf<Recipient, PermanentPhrase> Who { get; set; }
}
