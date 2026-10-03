namespace Glyphotype.Tests.Grammar;

// Exactly-one-of choices: the generic OneOf primitives, a concrete GlyphOneOf, and a pure OneOf alias.

/// <summary><see cref="OneOf{T1,T2}"/> over two enums.</summary>
public class GreetsUs : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Greeter), "greets us at the door"];

    public OneOf<Animal?, Person?> Greeter { get; set; }
}

[Dependent]
public class TheAnimal : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Animal)];

    public Animal Animal { get; set; }
}

[Dependent]
public class ThePerson : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Person)];

    public Person Person { get; set; }
}

/// <summary>A literal-only glyph with no <see cref="Glyph.Nibs"/> override: matched by its friendly-cased type name.</summary>
[Dependent]
public class OurNeighbor : Glyph
{
}

/// <summary><see cref="OneOf{T1,T2,T3}"/> over three nested glyphs.</summary>
public class KnocksOnTheDoor : Glyph
{
    public override Nib[] Nibs => [Prop(Visitor), "knocks on the door"];

    public OneOf<TheAnimal, ThePerson, OurNeighbor> Visitor { get; set; }
}

/// <summary>A concrete <see cref="GlyphOneOf"/>: alternatives with names of their own.</summary>
[Dependent]
public class Treat : GlyphOneOf
{
    public Food? Food { get; set; }
    public Fruit? Fruit { get; set; }
}

public class OffersTreat : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Person), "offers the", Prop(Animal), Alt("a", "an", "some"), Prop(Treat)];

    public Person Person { get; set; }
    public Animal Animal { get; set; }
    public Treat Treat { get; set; }
}

/// <summary>A pure <see cref="OneOf{T1,T2}"/> alias: naming it is all it takes to make it top-level.</summary>
public class DayHeading : OneOf<Weekday?, Holiday?>
{
}
