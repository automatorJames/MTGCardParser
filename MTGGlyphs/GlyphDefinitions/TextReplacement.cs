namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"one color word with another": one kind of word a text-changing effect swaps.</summary>
/// <exampledoc>Sleight of Mind</exampledoc>
/// <examplecapture>one color word with another</examplecapture>
[Dependent]
public class TextReplacement : Glyph
{
    public override Nib[] Nibs => ["one", Prop(Kind), "with another"];

    public TextWordKind Kind { get; set; }
}
