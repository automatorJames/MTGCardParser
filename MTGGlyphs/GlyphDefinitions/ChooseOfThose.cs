namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player picking some of a group just revealed, searched for, separated or targeted: "an opponent chooses one of those piles", "target opponent chooses two of those cards", "you choose one of them", "that player chooses one of those creatures" - or picking and sacrificing it at once: "their controller chooses and sacrifices one of them".</summary>
/// <exampledoc>Gifts Ungiven</exampledoc>
/// <examplecapture>chooses two of those cards</examplecapture>
public class ChooseOfThose : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Pattern("chooses?"), Prop(AndSacrifices), Prop(Count), "of", Alt("those piles", "their piles", "those cards", "those creatures", "them")];

    [Optional]
    [RegexPattern("and sacrifices?")]
    public bool? AndSacrifices { get; set; }
    public Quantity Count { get; set; }
}
