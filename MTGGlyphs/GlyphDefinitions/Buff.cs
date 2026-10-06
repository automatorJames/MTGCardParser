namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Something a permanent can get or have: a power/toughness change ("+1/+1"), a keyword ("flying"), becoming another type, or attacking despite defender.</summary>
/// <exampledoc>Blessing</exampledoc>
/// <examplecapture>+1/+1</examplecapture>
[Dependent]
public class Buff : GlyphOneOf
{
    public TransformedType TransformedType { get; set; }
    public PowerToughnessMod PowerToughnessModification { get; set; }
    public CanAttackAsThoughDidntHaveDefender CanAttackAsThoughDidntHaveDefender { get; set; }
    public Keyword? Keyword { get; set; }
}