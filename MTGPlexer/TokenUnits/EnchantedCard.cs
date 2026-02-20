namespace MTGPlexer.TokenUnits;

public class EnchantedCard : TokenUnit
{
    public override Snippet[] Snippets => ["enchanted", Prop(CardOrCreatureType), Prop(PermanentVerb), Prop(Buff)];

    public OneOf<CardType, CreatureType> CardOrCreatureType{ get; set; }
    public PermanentVerb? PermanentVerb { get; set; }   
    public Buff Buff { get; set; }
}