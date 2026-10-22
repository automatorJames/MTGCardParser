namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A power/toughness box on its own line, as in a level up bracket: "4/4" - the creature's base power and toughness at those levels.</summary>
/// <exampledoc>Beastbreaker of Bala Ged</exampledoc>
/// <examplecapture>4/4</examplecapture>
public class PowerToughnessBox : Glyph
{
    public override Nib[] Nibs => [Prop(Stats)];

    public PowerToughness Stats { get; set; }
}
