namespace MTGGlyphs.GlyphDefinitions;

[Dependent]
public class TransformedType : Glyph
{
    public override Nib[] Nibs => ["it's", Pattern("an?"), Prop(CardType)];

    [JoinedBy(Joiner.Space)]
    public CompoundOf<CardType> CardType { get; set; }
}