namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// "lands", "nonblack creatures", "nonartifact, nonblack creatures", "swamps", "tapped blue creatures they control", "creatures on the battlefield", "creatures with flying", "creatures with flying your opponents control", "creatures you control with power 2 or less", "creatures you control of the chosen type", "creatures of the chosen type on the battlefield", "creatures named {this} on the battlefield", "artifacts, creatures, and lands", "artifacts, legendaries, and/or sagas", "white and/or blue creatures", "creature they control that's attacking you", "creatures that share a creature type with it", "creatures that entered this turn", "creatures that attacked this turn", "creatures you control that are a wolf or a werewolf", "creatures you control that are enchanted", "auras attached to it", "permanents other than this equipment", "tokens created with this enchantment", "creature that convoked it", "creatures banded with it": permanents of one kind or several (card type, land type or creature type), optionally narrowed
/// before ("nonblack", or a comma-joined run "nonland, nontoken") or after ("with flying", "of the chosen color", "that's attacking you", "that share a creature type with it", "that entered this turn", "that dealt damage to you this turn", "that's a wolf or a werewolf", "that are equipped", "attached to it", "created with this enchantment", "that convoked it", "banded with it"), optionally by who controls them (a keyword qualifier may come first: "creatures without flying your opponents control"; a chosen aspect or name may too: "permanents of the chosen color your opponents control"), or anywhere on the battlefield, and optionally excluding this one. The kind is a referent, so a later "they" or "those creatures" refers to
/// them (see <see cref="ThatCard"/>).
/// </summary>
/// <exampledoc>Magnetic Mountain</exampledoc>
/// <examplecapture>tapped blue creatures they control</examplecapture>
[Dependent]
public class Permanents : Glyph
{
    public override Nib[] Nibs => [Prop(Qualifiers), Prop(Kind), Prop(PreNarrowed), Prop(Controller), Prop(OtherThan), Prop(Narrowed), Prop(InCombat)];

    [Optional]
    public OneOf<QualifierWords, CompoundOf<TargetQualifier>> Qualifiers { get; set; }
    [Referent]
    [Plural]
    public OneOf<PluralKindsAndOr, ManyOf<PluralKind>, PluralKind> Kind { get; set; }
    [Optional]
    public OneOf<ChosenAspect?, NamedThis> PreNarrowed { get; set; }
    [Optional]
    public OneOf<ControlledBy, OnTheBattlefield, KeywordedControlledBy> Controller { get; set; }
    [Optional]
    public OtherThanThis OtherThan { get; set; }
    [Optional]
    public OneOf<WithQualifier, ChosenAspect?> Narrowed { get; set; }
    [Optional]
    public OneOf<ThatsInCombat, SharesCreatureType, OneOf<EnteredThisTurn, ThatsOneOfKinds, OneOf<AttachedToReference, ThatDidThisTurn, OneOf<ThatAreAttached, CreatedWithThis, OneOf<ThatConvokedIt, BandedWith>>>>> InCombat { get; set; }
}
