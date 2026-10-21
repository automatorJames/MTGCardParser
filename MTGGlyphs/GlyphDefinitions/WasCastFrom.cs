namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Condition on where a spell was cast from: "this spell was cast from a graveyard", "{this} was cast from exile". (The active "you cast it from your hand" parses as SubjectDoes + PlayCard.)</summary>
/// <exampledoc>Increasing Devotion</exampledoc>
/// <examplecapture>this spell was cast from a graveyard</examplecapture>
[Dependent]
public class WasCastFrom : Glyph
{
    public override Nib[] Nibs => [Prop(Spell), "was cast from", Prop(Zone)];

    public OneOf<This, It, ThisObject> Spell { get; set; }
    public OneOf<PlayersZone, NonBattlefieldZone?> Zone { get; set; }
}
