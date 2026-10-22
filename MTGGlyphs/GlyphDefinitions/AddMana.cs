namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A mana ability's effect: "add {g}", "add {c}{c}{c}", "add {b} or {r}", "add one mana of any color", "add two mana in any combination of colors", "add an amount of {g} equal to this creature's power", or extra mana from a mana trigger, "its controller adds an additional {g}". Also a predicate, so "you lose 1 life and add {b}" resolves.</summary>
/// <exampledoc>Llanowar Elves</exampledoc>
/// <examplecapture>add {g}</examplecapture>
public class AddMana : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Alt("add", "adds"), Prop(Additional), Prop(Mana)];

    [Optional]
    [RegexPattern("an additional")]
    public bool? Additional { get; set; }
    public OneOf<ManyOf<ManaCost>, ManaCost, OneOf<ManaOfAnyColor, ManaInAnyCombination, AmountOfMana>> Mana { get; set; }
}
