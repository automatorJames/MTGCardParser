namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"to {this}", "to any target", "to creatures you control", "to you and permanents you control": what a prevention effect protects - a recipient, a group of permanents, or a player together with a group.</summary>
/// <exampledoc>Samite Healer</exampledoc>
/// <examplecapture>to any target</examplecapture>
[Dependent]
public class ProtectedRecipient : Glyph
{
    public override Nib[] Nibs => ["to", Prop(Who)];

    public OneOf<PlayerAndPermanents, ProtectedObject> Who { get; set; }
}
