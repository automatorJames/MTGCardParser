namespace MTGGlyphs.GlyphDefinitions;

[Dependent]
public class TransformedType : Glyph
{
    public override Nib[] Nibs => ["it's an?", Prop(CardType)];

    public CompoundOf<CardType> CardType { get; set; }
}