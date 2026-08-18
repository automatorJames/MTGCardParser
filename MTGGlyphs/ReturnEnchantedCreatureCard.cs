namespace MTGGlyphs;

public class ReturnEnchantedCreatureCard : Glyph
{
    public override Nib[] Nibs => ["return enchanted creature card", Prop(ToTheBattlefieldUnderControl)];

    public ToTheBattlefieldUnderControl ToTheBattlefieldUnderControl { get; set; }
}