namespace MTGGlyphs.GlyphDefinitions;

public class WithPowerToughnessEqual : Glyph
{
    public override Nib[] Nibs => ["with", Prop(PowerAndOrToughness), Opt("each"), "equal to", Prop(EquivalentToMeasurement)];

    public PowerAndOrToughness PowerAndOrToughness { get; set; }
    public EquivalentToMeasurement EquivalentToMeasurement { get; set; }
}
