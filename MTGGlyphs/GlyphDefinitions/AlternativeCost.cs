namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An alternative cost: "you may sacrifice two mountains rather than pay this spell's mana cost", "you may pay {1}{w} rather than pay this spell's mana cost", for other spells, "you may pay {w}{u}{b}{r}{g} rather than pay the mana cost for spells you cast", for a keyword ability's cost, "you may pay {0} rather than pay the echo cost for permanents you control", or as an instruction, "pay life equal to its mana value rather than pay its mana cost"; perhaps only in some game state, stated after it ("… if there are thirteen or more creatures on the battlefield").</summary>
/// <exampledoc>Fireblast</exampledoc>
/// <examplecapture>you may sacrifice two mountains rather than pay this spell's mana cost</examplecapture>
public class AlternativeCost : Glyph
{
    public override Nib[] Nibs => [Opt("you may"), Opt("pay"), Prop(Cost), Prop(AdditionalCost), Alt("rather than pay", "rather than paying"), Prop(Replaced), Prop(Provided)];

    public Cost Cost { get; set; }
    [Optional]
    public AndCost AdditionalCost { get; set; }
    public OneOf<ReplacedOwnCost?, CostForSpells, ReplacedKeywordCost> Replaced { get; set; }
    [Optional]
    public IfGameState Provided { get; set; }
}
