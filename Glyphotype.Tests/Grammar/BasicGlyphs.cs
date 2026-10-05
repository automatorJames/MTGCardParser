namespace Glyphotype.Tests.Grammar;

// Single-level glyphs: literal nibs, the nib helpers, and each scalar property kind.

/// <summary>Literal nibs, <c>Alt()</c>, plain enums, an enum synonym, and a multi-word enum member.</summary>
public class AnimalRests : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Animal), Alt("sleeps", "naps"), "in the", Prop(Place)];

    public Animal Animal { get; set; }
    public Place Place { get; set; }
}

/// <summary><c>Opt()</c>: an optional literal.</summary>
public class AnimalEats : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Animal), "eats", Opt("some"), Prop(Food)];

    public Animal Animal { get; set; }
    public Food Food { get; set; }
}

/// <summary>A <c>bool</c> property: true exactly when its pattern is present.</summary>
public class AnimalWalks : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Animal), "walks", Prop(Quietly), "to the", Prop(Place)];

    public Animal Animal { get; set; }
    [RegexPattern("quietly")] public bool Quietly { get; set; }
    public Place Place { get; set; }
}

/// <summary>An <c>int</c> property with the default pattern, and an <see cref="OptionalPluralAttribute"/> enum.</summary>
public class FruitInBowl : Glyph
{
    public override Nib[] Nibs => ["there", Alt("is", "are"), Prop(Count), Prop(Fruit), "in the bowl"];

    public int Count { get; set; }
    public Fruit Fruit { get; set; }
}

/// <summary>Literal text full of regex metacharacters: matched exactly as written, since a string nib is literal (see <see cref="Nib"/>).</summary>
public class AsksARiddle : Glyph
{
    public override Nib[] Nibs => ["the teacher asks what is 2+2?"];
}

/// <summary><c>[OptionalPlural]</c> on a property: its vocabulary's members, singular or plural, at just this property.</summary>
public class FeedAll : Glyph
{
    public override Nib[] Nibs => ["feed all the", Prop(Animal), "before noon"];

    [OptionalPlural]
    public Animal Animal { get; set; }
}
