namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A prevention effect: "prevent the next 2 damage that would be dealt to target creature this turn", "prevent all damage that would be dealt to {this} by artifact creatures". How much (the next N, or all), whether only combat damage, what it protects, from which sources, and whether it lasts only this turn.</summary>
/// <exampledoc>Samite Healer</exampledoc>
/// <examplecapture>prevent the next 1 damage that would be dealt to any target this turn</examplecapture>
public class PreventDamage : Glyph
{
    public override Nib[] Nibs => ["prevent", Prop(Amount), Prop(Combat), "damage that would be dealt to", Prop(Protected), Prop(Source), Prop(ThisTurn)];

    public PreventionAmount Amount { get; set; }
    [RegexPattern("combat")]
    public bool Combat { get; set; }
    public OneOf<Recipient, PermanentPhrase> Protected { get; set; }
    [Optional]
    public DamageSource Source { get; set; }
    [RegexPattern("this turn")]
    public bool ThisTurn { get; set; }
}
