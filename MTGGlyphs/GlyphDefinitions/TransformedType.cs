namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"it's an enchantment": a permanent being, or becoming, a type.</summary>
/// <exampledoc>Copy Artifact</exampledoc>
/// <examplecapture>it's an enchantment</examplecapture>
[Dependent]
public class TransformedType : Glyph
{
    public override Nib[] Nibs => ["it's", Pattern("an?"), Prop(CardType)];

    [JoinedBy(Joiner.Space)]
    public CompoundOf<CardType> CardType { get; set; }
}