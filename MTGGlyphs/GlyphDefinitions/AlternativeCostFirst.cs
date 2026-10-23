namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An alternative cost stated with the replaced cost first: "rather than pay this spell's mana cost, you may have an opponent gain 3 life". The cost is whatever instruction follows; usually under a condition ("if you control a forest, …").</summary>
/// <exampledoc>Invigorate</exampledoc>
/// <examplecapture>rather than pay this spell's mana cost, you may have an opponent gain 3 life</examplecapture>
public class AlternativeCostFirst : Glyph
{
    public override Nib[] Nibs => ["rather than pay", Prop(Replaced), ",", Prop(Cost)];

    public OneOf<ReplacedOwnCost?, CostForSpells> Replaced { get; set; }
    public DynamicGlyph Cost { get; set; }
}
