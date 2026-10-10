namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A set base power and/or toughness: "base power 0", "base power and toughness 0/2".</summary>
/// <exampledoc>Singing Tree</exampledoc>
/// <examplecapture>base power 0</examplecapture>
[Dependent]
public class BasePowerToughness : Glyph
{
    public override Nib[] Nibs => ["base", Prop(Stat), Pattern("[0-9x]+(/[0-9x]+)?")];

    public PowerAndOrToughness Stat { get; set; }
}
