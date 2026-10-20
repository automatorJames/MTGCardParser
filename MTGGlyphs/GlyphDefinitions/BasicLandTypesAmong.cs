namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"basic land type among lands you control" (domain): how many basic land types a player's lands span.</summary>
/// <exampledoc>Stratadon</exampledoc>
/// <examplecapture>basic land type among lands you control</examplecapture>
[Dependent]
public class BasicLandTypesAmong : Glyph
{
    public override Nib[] Nibs => ["basic land type among", Prop(Lands)];

    public Permanents Lands { get; set; }
}
