namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent's card named by where it is now: "this card from your graveyard", as unearth's reminder returns it.</summary>
/// <exampledoc>Rotting Rats</exampledoc>
/// <examplecapture>this card from your graveyard</examplecapture>
[Dependent]
public class PermanentFromZone : Glyph
{
    public override Nib[] Nibs => [Prop(Card), "from", Prop(Zone)];

    public PermanentPhrase Card { get; set; }
    public PlayersZone Zone { get; set; }
}
