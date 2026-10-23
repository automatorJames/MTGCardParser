namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A variable amount of one kind of mana: "an amount of {g} equal to this creature's power", "an amount of {c} equal to that spell's mana value", "an amount of mana of that color equal to your devotion to that color".</summary>
/// <exampledoc>Viridian Joiner</exampledoc>
/// <examplecapture>an amount of {g} equal to this creature's power</examplecapture>
[Dependent]
public class AmountOfMana : Glyph
{
    public override Nib[] Nibs => ["an amount of", Prop(Mana), Prop(Amount)];

    public OneOf<ManaCost, ManaOfChoice> Mana { get; set; }
    public EqualTo Amount { get; set; }
}
