namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"destroy all lands": destroys every permanent of a card type. The type is a referent, for a later "they".</summary>
/// <exampledoc>Armageddon</exampledoc>
/// <examplecapture>destroy all lands</examplecapture>
public class DestroyAllCardType : Glyph
{
    public override Nib[] Nibs => ["destroy all", Plural(Prop(CardType))];

    [Referent]
    [Plural]
    public CardType CardType { get; set; }
}