namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Several targeted cards of different kinds from one zone: "up to one target instant card and up to one target sorcery card from your graveyard", "up to one target artifact card and up to one target enchantment card from your graveyard".</summary>
/// <exampledoc>Pull from the Deep</exampledoc>
/// <examplecapture>up to one target instant card and up to one target sorcery card from your graveyard</examplecapture>
[Dependent]
public class TargetCardsOfKindsInZone : Glyph
{
    public override Nib[] Nibs => [Prop(Cards), "from", Prop(Zone)];

    public ManyOf<TargetedCardOfKind> Cards { get; set; }
    public OneOf<PlayersZone, NonBattlefieldZone?> Zone { get; set; }
}
