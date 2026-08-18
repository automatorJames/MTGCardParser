namespace MTGGlyphs;

public class EnchantedCard : Glyph
{
    public override Nib[] Nibs => ["enchanted", Prop(CardOrCreatureType), Prop(PermanentVerb), Prop(Buff)];

    public OneOf<CardType, CreatureType> CardOrCreatureType{ get; set; }
    public PermanentVerb? PermanentVerb { get; set; }   
    public Buff Buff { get; set; }
}