namespace MTGGlyphs.GlyphDefinitions;

public class DestroyAllCardType : Glyph
{
    public override Nib[] Nibs => ["destroy all", Plural(Prop(CardType))];

    [Referent, Plural]
    public CardType CardType { get; set; }
}