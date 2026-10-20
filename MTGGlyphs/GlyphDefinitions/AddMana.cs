namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A mana ability's effect: "add {g}", "add {c}{c}{c}", "add {b} or {r}", "add one mana of any color". Also a predicate, so "you lose 1 life and add {b}" resolves.</summary>
/// <exampledoc>Llanowar Elves</exampledoc>
/// <examplecapture>add {g}</examplecapture>
public class AddMana : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["add", Prop(Mana)];

    public OneOf<ManyOf<ManaCost>, ManaCost, ManaOfAnyColor> Mana { get; set; }
}
