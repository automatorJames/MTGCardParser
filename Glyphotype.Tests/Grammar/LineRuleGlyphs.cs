namespace Glyphotype.Tests.Grammar;

// How much of a line a glyph may (or must) cover, which glyph gets first try, and the {this} placeholder.

/// <summary>A bare <c>"."</c> nib: this glyph deliberately spans two clauses.</summary>
public class MorningRoutine : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Animal), "wakes up", ".", "then it eats", Prop(Food)];

    public Animal Animal { get; set; }
    public Food Food { get; set; }
}

/// <summary>A literal nib with a period inside it: split around the period, so it spans two clauses just as <see cref="MorningRoutine"/> does.</summary>
public class EveningRoutine : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Animal), "goes to bed. then it dreams of", Prop(Food)];

    public Animal Animal { get; set; }
    public Food Food { get; set; }
}

/// <summary><see cref="AllowPartialClauseMatchAttribute"/>: may match the start of a clause and leave the rest.</summary>
[AllowPartialClauseMatch]
public class Greeting : Glyph
{
    public override Nib[] Nibs => ["good", Prop(TimeOfDay)];

    public TimeOfDay TimeOfDay { get; set; }
}

/// <summary>General: any person opening any building. Its longer regex would normally be tried before <see cref="BakerOpensTheShop"/>.</summary>
public class OpensBuilding : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Person), "opens the", Prop(Building)];

    public Person Person { get; set; }
    public Building Building { get; set; }
}

/// <summary><see cref="TokenizationOrderAttribute"/>: tried ahead of <see cref="OpensBuilding"/>, which would otherwise claim this sentence.</summary>
[TokenizationOrder(0)]
public class BakerOpensTheShop : Glyph
{
    public override Nib[] Nibs => ["the baker opens the shop"];
}

/// <summary>The <c>{this}</c> placeholder, which stands for the document's own name.</summary>
public class ThisBarksAt : Glyph
{
    public override Nib[] Nibs => ["{this} barks at the", Prop(Person)];

    public Person Person { get; set; }
}
