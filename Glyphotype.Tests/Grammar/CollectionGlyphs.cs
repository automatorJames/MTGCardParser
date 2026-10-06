namespace Glyphotype.Tests.Grammar;

// Several-of-a-kind: ManyOf (a list with a conjunction) and CompoundOf (a joined list without one).

/// <summary><see cref="ManyOf{T}"/>: "x and y", "x, y, and z", "x or y".</summary>
public class AnimalLikes : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Animal), "likes", Prop(Foods)];

    public Animal Animal { get; set; }
    public ManyOf<Food> Foods { get; set; }
}

/// <summary>A <see cref="CompoundOf{T}"/> alias whose class-level <see cref="JoinedByAttribute"/> makes it space-joined ("big old").</summary>
[Dependent]
[JoinedBy(Joiner.Space)]
public class TraitList : CompoundOf<Trait>
{
}

/// <summary>Uses <see cref="TraitList"/> as declared: space-joined.</summary>
public class WeHaveAn : Glyph
{
    public override Nib[] Nibs => ["we have", Pattern("an?"), Prop(Traits), Prop(Animal)];

    public TraitList Traits { get; set; }
    public Animal Animal { get; set; }
}

/// <summary>A property-level <see cref="JoinedByAttribute"/> overriding <see cref="TraitList"/>'s class-level one: comma-joined ("big, friendly").</summary>
public class FamilyAdopts : Glyph
{
    public override Nib[] Nibs => ["the family adopts", Pattern("an?"), Prop(Traits), Prop(Animal)];

    [JoinedBy(Joiner.CommaSpace)] public TraitList Traits { get; set; }
    public Animal Animal { get; set; }
}

/// <summary>
/// A top-level <see cref="CompoundOf{T}"/> alias with the default comma joiner, e.g. a line reading "flour, sugar, eggs".
/// Also carries a get-only convenience property, which a pure alias may declare since it's never regex-bound.
/// </summary>
public class ShoppingList : CompoundOf<Ingredient>
{
    public bool NeedsEggs => Items.Contains(Ingredient.Eggs);
}
