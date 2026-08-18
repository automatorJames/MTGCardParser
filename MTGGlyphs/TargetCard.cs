namespace MTGGlyphs;

public class TargetCard : Glyph
{
    public override Nib[] Nibs => ["target", Prop(CardType)];

    public CardType CardType { get; set; }
}