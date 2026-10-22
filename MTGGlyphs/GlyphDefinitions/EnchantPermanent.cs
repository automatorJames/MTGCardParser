namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An aura's enchant ability: "enchant land", "enchant creature", "enchant creature you control", "enchant player", "enchant artifact or creature", "enchant nonland permanent", "enchant red or green creature", "enchant creature without flying", "enchant creature with power 3 or less", optionally outside the battlefield ("enchant creature card in a graveyard"), optionally followed by its reminder text ("(target a creature as you cast this").</summary>
/// <exampledoc>Consecrate Land</exampledoc>
/// <examplecapture>enchant land</examplecapture>
public class EnchantPermanent : Glyph
{
    public override Nib[] Nibs => ["enchant", Prop(Qualifiers), Prop(Kind), Prop(Controller), Prop(With), Prop(CardOutsideBattlefield), Prop(Reminder)];

    [Optional]
    public OneOf<ManyOf<TargetQualifier>, QualifierRun> Qualifiers { get; set; }
    public OneOf<ManyOf<PermanentKind>, PermanentKind, PlayerIdentity?> Kind { get; set; }
    [Optional]
    public ControlledBy Controller { get; set; }
    [Optional]
    public WithQualifier With { get; set; }
    [Optional]
    public CardOutsideBattlefield CardOutsideBattlefield { get; set; }
    [Optional]
    public ReminderText Reminder { get; set; }
}