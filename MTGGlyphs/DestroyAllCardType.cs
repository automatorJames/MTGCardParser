namespace MTGGlyphs;

public class DestroyAllCardType : Glyph
{
    public override Nib[] Nibs => ["destroy all", Prop(CardType, Proptions.Plural)];

    public CardType CardType { get; set; }
}