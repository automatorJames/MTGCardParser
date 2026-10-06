namespace Glyphotype.BackReferences;

// The standard pronouns: back-references every grammar can use as they are, without declaring its own. Each is
// [Dependent] - a pronoun only means anything inside the phrase around it - and a plain BackReference, so it binds to
// the most recent referent of its number whatever its kind. A back-reference that names its referent's kind ("that
// animal", a BackReference<Animal>) belongs to the grammar whose vocabulary that kind is.
//
// Bare "this" and "that" aren't here: as words they're far more often determiners ("that animal", "this way") or
// conjunctions than pronouns. "They" is plural; where a grammar uses it for one person, it declares a
// BackReference<T> of that person's kind matching "they", for the places only a person can be meant.

/// <summary>"it": a singular pronoun, as subject or object.</summary>
[Dependent]
[Singular]
public class It : BackReference
{
    public override Nib[] Nibs => ["it"];
}

/// <summary>"its": a singular possessive, as in "its owner".</summary>
[Dependent]
[Singular]
public class Its : BackReference
{
    public override Nib[] Nibs => ["its"];
}

/// <summary>"itself": a singular reflexive.</summary>
[Dependent]
[Singular]
public class Itself : BackReference
{
    public override Nib[] Nibs => ["itself"];
}

/// <summary>"they": a plural pronoun, as subject.</summary>
[Dependent]
[Plural]
public class They : BackReference
{
    public override Nib[] Nibs => ["they"];
}

/// <summary>"them": a plural pronoun, as object.</summary>
[Dependent]
[Plural]
public class Them : BackReference
{
    public override Nib[] Nibs => ["them"];
}

/// <summary>"their": a plural possessive, as in "their owners".</summary>
[Dependent]
[Plural]
public class Their : BackReference
{
    public override Nib[] Nibs => ["their"];
}

/// <summary>"themselves": a plural reflexive.</summary>
[Dependent]
[Plural]
public class Themselves : BackReference
{
    public override Nib[] Nibs => ["themselves"];
}

/// <summary>"these": a plural demonstrative pronoun, standing alone.</summary>
[Dependent]
[Plural]
public class These : BackReference
{
    public override Nib[] Nibs => ["these"];
}

/// <summary>"those": a plural demonstrative pronoun, standing alone.</summary>
[Dependent]
[Plural]
public class Those : BackReference
{
    public override Nib[] Nibs => ["those"];
}
