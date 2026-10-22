namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A prevention effect: "prevent the next 2 damage that would be dealt to target creature this turn", "prevent all damage that would be dealt this turn to creatures you control", "prevent all noncombat damage that would be dealt to other creatures you control", "prevent all combat damage that would be dealt by target creature this turn". How much (the next N, or all), whether only combat or noncombat damage, what it protects (if limited), from which sources, and whether it lasts only this turn (said before or after what it protects or the sources). Also a predicate, so "you may prevent …" resolves.</summary>
/// <exampledoc>Samite Healer</exampledoc>
/// <examplecapture>prevent the next 1 damage that would be dealt to any target this turn</examplecapture>
public class PreventDamage : Glyph, IPredicate
{
    public override Nib[] Nibs => ["prevent", Prop(Amount), Prop(Kind), "damage that would be dealt", Prop(ThisTurnLeading), Prop(Protected), Prop(ThisTurnFirst), Prop(Source), Prop(ThisTurn)];

    public PreventionAmount Amount { get; set; }
    [Optional]
    public DamageKind? Kind { get; set; }
    [RegexPattern("this turn")]
    public bool ThisTurnLeading { get; set; }
    [Optional]
    public OneOf<DealtToAndBy, ProtectedRecipient> Protected { get; set; }
    [RegexPattern("this turn")]
    public bool ThisTurnFirst { get; set; }
    [Optional]
    public DamageSource Source { get; set; }
    [RegexPattern("this turn")]
    public bool ThisTurn { get; set; }
}
