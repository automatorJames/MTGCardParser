namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Mana of some kind spent on a spell or a payment, as a "for each" counts it: "{s} spent to cast this spell", "{g}{g} spent to cast it", "{b} or {r} spent this way", "mana from a treasure that was spent to cast this spell", or any mana ("mana spent to cast it").</summary>
/// <exampledoc>Clay Champion</exampledoc>
/// <examplecapture>{g}{g} spent to cast it</examplecapture>
[Dependent]
public class ManaSpent : Glyph
{
    public override Nib[] Nibs => [Prop(Mana), Opt("that was"), "spent", Prop(Use)];

    public OneOf<ManaCost, ManyOf<ManaCost>, ManaFromSource, AnyMana> Mana { get; set; }
    public OneOf<SpellPaidFor, ThisWay> Use { get; set; }
}
