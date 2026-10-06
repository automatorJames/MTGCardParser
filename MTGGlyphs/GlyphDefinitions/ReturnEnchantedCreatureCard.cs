namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"return enchanted creature card to the battlefield under your control …": an aura bringing back the card it enchants.</summary>
/// <exampledoc>Animate Dead</exampledoc>
/// <examplecapture>return enchanted creature card to the battlefield under your control and attach {this} to it</examplecapture>
public class ReturnEnchantedCreatureCard : Glyph
{
    public override Nib[] Nibs => ["return enchanted creature card", Prop(ToTheBattlefieldUnderControl)];

    public ToTheBattlefieldUnderControl ToTheBattlefieldUnderControl { get; set; }
}