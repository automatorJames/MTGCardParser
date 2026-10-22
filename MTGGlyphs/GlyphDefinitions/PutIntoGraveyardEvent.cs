namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"is put into your graveyard from the battlefield", "is put into an opponent's graveyard from anywhere", "is put into an opponent's graveyard": a permanent or card going to a particular player's graveyard (the older wording of "dies", and its kin). "a graveyard" stays with the PermanentEvent members.</summary>
/// <exampledoc>Enduring Renewal</exampledoc>
/// <examplecapture>is put into your graveyard from the battlefield</examplecapture>
[Dependent]
public class PutIntoGraveyardEvent : Glyph
{
    public override Nib[] Nibs => ["is put into", Prop(Whose), "graveyard", Prop(Origin)];

    public Whose Whose { get; set; }
    [Optional]
    public GraveyardOrigin? Origin { get; set; }
}
