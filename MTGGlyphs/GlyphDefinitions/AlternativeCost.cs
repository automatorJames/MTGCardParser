namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An alternative cost: "you may sacrifice two mountains rather than pay this spell's mana cost", "you may pay {1}{w} rather than pay this spell's mana cost", for other spells, "you may pay {w}{u}{b}{r}{g} rather than pay the mana cost for spells you cast", or as an instruction, "pay life equal to its mana value rather than pay its mana cost".</summary>
/// <exampledoc>Fireblast</exampledoc>
/// <examplecapture>you may sacrifice two mountains rather than pay this spell's mana cost</examplecapture>
public class AlternativeCost : Glyph
{
    public override Nib[] Nibs => [Opt("you may"), Opt("pay"), Prop(Cost), Prop(Also), Alt("rather than pay", "rather than paying"), Prop(Replaced)];

    public Cost Cost { get; set; }
    [Optional]
    public AndCost Also { get; set; }
    public OneOf<ReplacedOwnCost?, CostForSpells> Replaced { get; set; }
}
