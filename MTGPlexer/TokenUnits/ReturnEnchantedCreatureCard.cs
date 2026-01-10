namespace MTGPlexer.TokenUnits;

public class ReturnEnchantedCreatureCard : TokenUnit
{
    protected override Snippet[] Snippets => ["return enchanted creature card", Prop(ToTheBattlefieldUnderControl)];

    public ToTheBattlefieldUnderControl ToTheBattlefieldUnderControl { get; set; }
}