namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A prevention effect naming the damage's source as the subject: "prevent all damage that this creature would deal to snow creatures", "prevent all damage that sources of the color of your choice would deal this turn". The "damage that would be dealt by …" wording is <see cref="PreventDamage"/>.</summary>
/// <exampledoc>Goblin Furrier</exampledoc>
/// <examplecapture>prevent all damage that this creature would deal to snow creatures</examplecapture>
public class PreventDamageDealtBy : Glyph, IPredicate
{
    public override Nib[] Nibs => ["prevent", Prop(Amount), Prop(Kind), "damage that", Prop(Dealer), "would deal", Prop(ThisTurnFirst), Prop(Protected), Prop(ThisTurn)];

    public PreventionAmount Amount { get; set; }
    [Optional]
    public DamageKind? Kind { get; set; }
    public OneOf<Permanents, DamageSources, PermanentPhrase, ChosenSource> Dealer { get; set; }
    [RegexPattern("this turn")]
    public bool ThisTurnFirst { get; set; }
    [Optional]
    public ProtectedRecipient Protected { get; set; }
    [RegexPattern("this turn")]
    public bool ThisTurn { get; set; }
}
