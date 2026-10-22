namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player picking some of a group just revealed, searched for or separated: "an opponent chooses one of those piles", "target opponent chooses two of those cards", "you choose one of them".</summary>
/// <exampledoc>Gifts Ungiven</exampledoc>
/// <examplecapture>chooses two of those cards</examplecapture>
public class ChooseOfThose : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Pattern("chooses?"), Prop(Count), "of", Alt("those piles", "their piles", "those cards", "them")];

    public Quantity Count { get; set; }
}
