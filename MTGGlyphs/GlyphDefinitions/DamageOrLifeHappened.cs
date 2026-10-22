namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Damage or life changed without a named actor: "prevented this way", "lost this way", "dealt to you this turn".</summary>
/// <exampledoc>Discordant Spirit</exampledoc>
/// <examplecapture>dealt to you this turn</examplecapture>
[Dependent]
public class DamageOrLifeHappened : Glyph
{
    public override Nib[] Nibs => [Prop(Change), Prop(To), Prop(When)];

    public DamageOrLifeChange Change { get; set; }
    [Optional]
    public DamageRecipientPlayer To { get; set; }
    public DamageOrLifeWhen When { get; set; }
}
