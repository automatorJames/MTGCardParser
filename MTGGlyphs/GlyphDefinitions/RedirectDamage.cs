namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A redirection effect: "the next 1 damage that would be dealt to this creature this turn is dealt to target creature you control instead", "all damage that would be dealt to you is dealt to enchanted creature instead". How much (the next N, or all), whether only combat damage, what it would have hit, from which sources, whether only this turn, and where it goes instead.</summary>
/// <exampledoc>Lancers en-Kor</exampledoc>
/// <examplecapture>the next 1 damage that would be dealt to this creature this turn is dealt to target creature you control instead</examplecapture>
public class RedirectDamage : Glyph
{
    public override Nib[] Nibs => [Prop(Amount), Prop(Combat), "damage that would be dealt", Prop(Protected), Prop(ThisTurnFirst), Prop(Source), Prop(ThisTurn), "is dealt", Prop(NewRecipient), "instead"];

    public PreventionAmount Amount { get; set; }
    [RegexPattern("combat")]
    public bool Combat { get; set; }
    [Optional]
    public OneOf<DealtToAndBy, ProtectedRecipient> Protected { get; set; }
    [RegexPattern("this turn")]
    public bool ThisTurnFirst { get; set; }
    [Optional]
    public DamageSource Source { get; set; }
    [RegexPattern("this turn")]
    public bool ThisTurn { get; set; }
    public ProtectedRecipient NewRecipient { get; set; }
}
