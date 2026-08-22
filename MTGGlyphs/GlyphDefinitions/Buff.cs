namespace MTGGlyphs.GlyphDefinitions;

[Dependent]
public class Buff : GlyphOneOf
{
    public TransformedType TransformedType { get; set; }
    public PowerToughnessMod PowerToughnessModification { get; set; }
    public CanAttackAsThoughDidntHaveDefender CanAttackAsThoughDidntHaveDefender { get; set; }
    public Keyword? Keyword { get; set; }
}