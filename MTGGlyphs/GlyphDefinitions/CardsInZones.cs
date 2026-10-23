namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"cards in graveyards": cards in a public zone, as the sources of abilities.</summary>
/// <exampledoc>Ashes of the Abhorrent</exampledoc>
/// <examplecapture>cards in graveyards</examplecapture>
[Dependent]
public class CardsInZones : Glyph
{
    public override Nib[] Nibs => ["cards in", Plural(Prop(Zone))];

    public NonBattlefieldZone Zone { get; set; }
}
