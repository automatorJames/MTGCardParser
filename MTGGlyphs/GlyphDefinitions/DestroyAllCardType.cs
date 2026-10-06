namespace MTGGlyphs.GlyphDefinitions;

public class DestroyAllCardType : Glyph
{
    public override Nib[] Nibs => ["destroy all", Prop(CardType), Plural()];

    [Referent, Plural]
    public CardType CardType { get; set; }
}