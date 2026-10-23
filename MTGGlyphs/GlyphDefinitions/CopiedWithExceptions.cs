namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An object copied with exceptions to its copiable values: "it, except it's a 4/4 black zombie cat with no mana cost", "target creature, except it's an artifact in addition to its other types", "any creature on the battlefield, except it's 7/7", "it, except it isn't legendary", `target creature, except it has haste and "…"`, "it, except it enters with an additional +1/+1 counter on it", "that creature, except it's 1/1 and it's a nightmare …", "it, except its name is {this} and it's legendary …" (renamed copies), "target creature card in your graveyard with mana value x, except it has flying and this ability"; optionally for a duration ("a copy of target creature until end of turn, except …"). The comma is a literal here because a nested glyph can't open with one; "except it's" / "except it has" open the change, so the change alone never reads as a condition like "it's an angel".</summary>
/// <exampledoc>Adorned Pouncer</exampledoc>
/// <examplecapture>it, except it's a 4/4 black zombie cat with no mana cost</examplecapture>
[Dependent]
public class CopiedWithExceptions : Glyph
{
    public override Nib[] Nibs => [Prop(Original), Prop(Duration), ",", Prop(Change)];

    public OneOf<PermanentPhrase, IndefinitePermanent, OneOf<AnyObjectInZone, TargetCardInZone>> Original { get; set; }
    [Optional]
    public EffectDuration Duration { get; set; }
    public OneOf<CopyChangePair, CopyIsDescribed, OneOf<CopyHasAbility, CopyLegendary, OneOf<CopyGrantsQuoted, CopyEntersWithCounters, OneOf<CopyRenamed, CopyRenamedRetypedGranted>>>> Change { get; set; }
}
