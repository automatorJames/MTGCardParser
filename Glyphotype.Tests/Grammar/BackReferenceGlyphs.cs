namespace Glyphotype.Tests.Grammar;

// Back-references - the standard pronouns "it" and "they", and "that animal" - and the referents they're resolved to
// after tokenization.

/// <summary>A back-reference that also names its referent's kind, so it skips past a more recent referent of another kind.</summary>
[Dependent]
[Agreement(GrammaticalNumber.Singular, "animal")]
public class ThatAnimal : BackReference
{
    public override Nib[] Nibs => ["that animal"];
}

/// <summary>Back-references as a sentence's subject.</summary>
public class Rests : Glyph
{
    public override Nib[] Nibs => [Prop(Subject), Alt("sleeps", "sleep"), "all day"];

    public OneOf<It, They, ThatAnimal> Subject { get; set; }
}

/// <summary>Property-level <see cref="IntroducesAttribute"/>, with each referent's number and kind.</summary>
public class Befriends : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Animal), "befriends the", Prop(Person)];

    [Introduces(GrammaticalNumber.Singular, "animal")]
    public Animal Animal { get; set; }

    [Introduces(GrammaticalNumber.Singular, "person")]
    public Person Person { get; set; }
}

/// <summary>A plural referent, captured by an enum property.</summary>
public class Buys : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Person), "buys", Prop(Count), Prop(Fruit)];

    [Introduces(GrammaticalNumber.Singular, "person")]
    public Person Person { get; set; }

    public int Count { get; set; }

    [Introduces(GrammaticalNumber.Plural, "fruit")]
    public Fruit Fruit { get; set; }
}

/// <summary>
/// Class-level <see cref="IntroducesAttribute"/>: every match is a referent - and one with a back-reference inside it,
/// which has to be resolved before the phrase around it is introduced (it can't be its own antecedent).
/// </summary>
[Dependent]
[Introduces(GrammaticalNumber.Plural)]
public class FriendsOf : Glyph
{
    public override Nib[] Nibs => ["the friends of", Prop(Friend)];

    public It Friend { get; set; }
}

public class Meets : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Animal), "meets", Prop(Friends)];

    [Introduces(GrammaticalNumber.Singular, "animal")]
    public Animal Animal { get; set; }

    public FriendsOf Friends { get; set; }
}

/// <summary><see cref="RefersToAttribute"/>: the glyph binds its own pronoun, overriding the more recent referent a search would pick.</summary>
public class FollowsUntil : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Animal), "follows the", Prop(Person), "until", Prop(Follower), "rests"];

    public Animal Animal { get; set; }

    [Introduces(GrammaticalNumber.Singular, "person")]
    public Person Person { get; set; }

    [RefersTo(nameof(Animal))]
    public It Follower { get; set; }
}

/// <summary>A literal self-reference: the document's own name is a referent wherever it sits in a match's text.</summary>
public class Visits : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Person), "visits {this}"];

    [Introduces(GrammaticalNumber.Singular, "person")]
    public Person Person { get; set; }
}

/// <summary>A captured self-reference: the document as one of the things that can fill a property.</summary>
public class Feeds : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Person), "feeds", Prop(Fed)];

    [Introduces(GrammaticalNumber.Singular, "person")]
    public Person Person { get; set; }

    public OneOf<This, FriendsOf> Fed { get; set; }
}
