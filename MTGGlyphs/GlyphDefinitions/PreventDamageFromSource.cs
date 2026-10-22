namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A prevention effect naming the source first: "prevent all combat damage target creature would deal this turn", "prevent all damage a source of your choice would deal to you this turn", "prevent all combat damage non-soldier creatures would deal this turn". How much, which kind of damage, the source (a chosen source, one permanent, or a group), what it protects if limited, and whether it lasts only this turn.</summary>
/// <exampledoc>Azorius Ploy</exampledoc>
/// <examplecapture>prevent all combat damage target creature would deal this turn</examplecapture>
public class PreventDamageFromSource : Glyph, IPredicate
{
    public override Nib[] Nibs => ["prevent", Prop(Amount), Prop(Kind), "damage", Prop(Source), "would deal", Prop(Protected), Prop(ThisTurn)];

    public PreventionAmount Amount { get; set; }
    [Optional]
    public DamageKind? Kind { get; set; }
    public OneOf<ChosenSource, PermanentPhrase, Permanents> Source { get; set; }
    [Optional]
    public ProtectedRecipient Protected { get; set; }
    [RegexPattern("this turn")]
    public bool ThisTurn { get; set; }
}
