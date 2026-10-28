namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent's card named by where it is now: "this card from your graveyard", as unearth's reminder returns it; also a card named earlier ("the discarded card from your graveyard", "those cards from your graveyard") or a legend's short name ("akuta from your graveyard").</summary>
/// <exampledoc>Rotting Rats</exampledoc>
/// <examplecapture>this card from your graveyard</examplecapture>
[Dependent]
public class PermanentFromZone : Glyph
{
    public override Nib[] Nibs => [Prop(Card), "from", Prop(Zone)];

    public OneOf<PermanentPhrase, SacrificedPermanent, ThatCardPhrase, ShortName> Card { get; set; }
    public PlayersZone Zone { get; set; }
}
