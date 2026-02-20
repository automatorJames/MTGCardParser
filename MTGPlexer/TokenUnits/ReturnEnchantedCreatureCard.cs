namespace MTGPlexer.TokenUnits;

public class ReturnEnchantedCreatureCard : TokenUnit
{
    public override Snippet[] Snippets => ["return enchanted creature card", Prop(ToTheBattlefieldUnderControl)];

    public ToTheBattlefieldUnderControl ToTheBattlefieldUnderControl { get; set; }
}