namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Manifest dread's reminder, second sentence: "put one onto the battlefield face down as a 2/2 creature and the other into your graveyard".</summary>
/// <exampledoc>Break Down the Door</exampledoc>
/// <examplecapture>put one onto the battlefield face down as a 2/2 creature and the other into your graveyard</examplecapture>
public class ManifestDreadRule : Glyph
{
    public override Nib[] Nibs => ["put one onto the battlefield face down as a", Prop(Stats), "creature and the other into your graveyard"];

    public PowerToughness Stats { get; set; }
}
