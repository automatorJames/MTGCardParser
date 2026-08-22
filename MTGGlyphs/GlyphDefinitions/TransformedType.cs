namespace MTGGlyphs.GlyphDefinitions;

[Dependent]
public class TransformedType : Glyph
{
    public override Nib[] Nibs => ["an?", Prop(CardType)];

    public CompoundOf<CardType> CardType { get; set; }
}