namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Condition on where a spell was cast from: "this spell was cast from a graveyard", "{this} was cast from exile", "this spell wasn't cast from your hand", "this spell was cast from anywhere other than your hand". (The active "you cast it from your hand" parses as SubjectDoes + PlayCard.)</summary>
/// <exampledoc>Increasing Devotion</exampledoc>
/// <examplecapture>this spell was cast from a graveyard</examplecapture>
[Dependent]
public class WasCastFrom : Glyph
{
    public override Nib[] Nibs => [Prop(Spell), Prop(Polarity), "cast from", Prop(OtherThan), Prop(Zone)];

    public OneOf<This, It, ThisObject> Spell { get; set; }
    public PastPolarity Polarity { get; set; }
    [RegexPattern("anywhere other than")]
    public bool OtherThan { get; set; }
    public OneOf<PlayersZone, NonBattlefieldZone?> Zone { get; set; }
}
