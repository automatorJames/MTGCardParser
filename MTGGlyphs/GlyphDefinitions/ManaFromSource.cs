namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Mana produced by a kind of permanent: "mana from a treasure", "mana from a cave".</summary>
/// <exampledoc>Kalain, Reclusive Painter</exampledoc>
/// <examplecapture>mana from a treasure</examplecapture>
[Dependent]
public class ManaFromSource : Glyph
{
    public override Nib[] Nibs => ["mana from", Prop(Source)];

    public IndefinitePermanent Source { get; set; }
}
