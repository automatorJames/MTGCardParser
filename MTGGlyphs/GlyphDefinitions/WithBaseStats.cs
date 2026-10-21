namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"with base power and toughness 4/4": the base stats a permanent becomes, in an animation or transformation.</summary>
/// <exampledoc>Turn to Frog</exampledoc>
/// <examplecapture>with base power and toughness 1/1</examplecapture>
[Dependent]
public class WithBaseStats : Glyph
{
    public override Nib[] Nibs => ["with", Prop(Stats)];

    public BasePowerToughness Stats { get; set; }
}
