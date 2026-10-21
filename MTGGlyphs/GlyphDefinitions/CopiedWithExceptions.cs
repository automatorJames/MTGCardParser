namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An object copied with exceptions to its copiable values: "it, except it's a 4/4 black zombie cat with no mana cost", "target creature, except it's an artifact in addition to its other types", "any creature on the battlefield, except it's 7/7". The comma is a literal here because a nested glyph can't open with one; "except it's" / "except it has" open the change, so the change alone never reads as a condition like "it's an angel".</summary>
/// <exampledoc>Adorned Pouncer</exampledoc>
/// <examplecapture>it, except it's a 4/4 black zombie cat with no mana cost</examplecapture>
[Dependent]
public class CopiedWithExceptions : Glyph
{
    public override Nib[] Nibs => [Prop(Original), ",", Prop(Change)];

    public OneOf<PermanentPhrase, IndefinitePermanent, AnyObjectInZone> Original { get; set; }
    public OneOf<CopyIsDescribed, CopyHasAbility> Change { get; set; }
}
