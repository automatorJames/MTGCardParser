namespace Glyphotype.Tests.Grammar;

// Back-references - the standard pronouns "it" and "they", and "that animal", "that person" - and the referents they're
// resolved to after tokenization.

/// <summary>A back-reference of one kind: it skips past a more recent referent of another kind.</summary>
[Dependent]
public class ThatAnimal : BackReference<Animal>
{
    public override Nib[] Nibs => ["that animal"];
}

/// <summary>Another back-reference of one kind - which skips {this}, whose kind is <see cref="This"/>.</summary>
[Dependent]
public class ThatPerson : BackReference<Person>
{
    public override Nib[] Nibs => ["that person"];
}

/// <summary>Back-references as a sentence's subject.</summary>
public class Rests : Glyph
{
    public override Nib[] Nibs => [Prop(Subject), Alt("sleeps", "sleep"), "all day"];

    public OneOf<It, They, ThatAnimal> Subject { get; set; }
}

public class Waves : Glyph
{
    public override Nib[] Nibs => [Prop(Waver), "waves"];

    public ThatPerson Waver { get; set; }
}

/// <summary>Property-level <see cref="ReferentAttribute"/>s, each of its property's kind, with their numbers.</summary>
public class Befriends : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Animal), "befriends the", Prop(Person)];

    [Referent(GrammaticalNumber.Singular)]
    public Animal Animal { get; set; }

    [Referent(GrammaticalNumber.Singular)]
    public Person Person { get; set; }
}

/// <summary>A plural referent, captured by an enum property.</summary>
public class Buys : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Person), "buys", Prop(Count), Prop(Fruit)];

    [Referent(GrammaticalNumber.Singular)]
    public Person Person { get; set; }

    public int Count { get; set; }

    [Referent(GrammaticalNumber.Plural)]
    public Fruit Fruit { get; set; }
}

/// <summary>
/// Class-level <see cref="ReferentAttribute"/>: every match is a referent - and one with a back-reference inside it,
/// which has to be resolved before the phrase around it becomes a referent (it can't be its own antecedent).
/// </summary>
[Dependent]
[Referent(GrammaticalNumber.Plural)]
public class FriendsOf : Glyph
{
    public override Nib[] Nibs => ["the friends of", Prop(Friend)];

    public It Friend { get; set; }
}

public class Meets : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Animal), "meets", Prop(Friends)];

    [Referent(GrammaticalNumber.Singular)]
    public Animal Animal { get; set; }

    public FriendsOf Friends { get; set; }
}

/// <summary><see cref="RefersToAttribute"/>: the glyph binds its own pronoun, overriding the more recent referent a search would pick.</summary>
public class FollowsUntil : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Animal), "follows the", Prop(Person), "until", Prop(Follower), "rests"];

    public Animal Animal { get; set; }

    [Referent(GrammaticalNumber.Singular)]
    public Person Person { get; set; }

    [RefersTo(nameof(Animal))]
    public It Follower { get; set; }
}

/// <summary>A literal self-reference: the document's own name is a referent wherever it sits in a match's text.</summary>
public class Visits : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Person), "visits", Nib.This];

    [Referent(GrammaticalNumber.Singular)]
    public Person Person { get; set; }
}

/// <summary>A captured self-reference: the document as one of the things that can fill a property.</summary>
public class Feeds : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Person), "feeds", Prop(Fed)];

    [Referent(GrammaticalNumber.Singular)]
    public Person Person { get; set; }

    public OneOf<This, FriendsOf> Fed { get; set; }
}
