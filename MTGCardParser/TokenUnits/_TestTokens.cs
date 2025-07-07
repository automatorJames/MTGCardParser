namespace MTGCardParser.TokenUnits;

public class Condition : TokenUnitBase
{
    public RegexTemplate<Condition> RegexTemplate => new(nameof(PredicateStatement), nameof(FactStatement));

    public PredicateStatement PredicateStatement { get; set; }
    public FactStatement FactStatement { get; set; }

}


public class PredicateStatement : TokenUnitBase
{
    public RegexTemplate<PredicateStatement> RegexTemplate => new(nameof(Predicate));

    public Predicate? Predicate { get; set; }

}

public class FactStatement : TokenUnitBase
{
    public RegexTemplate<FactStatement> RegexTemplate => new(nameof(FactPart));

    public FactPart FactPart { get; set; }

}

public class FactPart : TokenUnitBase
{
    public RegexTemplate<FactPart> RegexTemplate => new(nameof(Fact), nameof(FactValue));

    public Fact? Fact { get; set; }
    public FactValue? FactValue { get; set; }

}



public enum Predicate
{
    [RegexPattern("as long as")]
    AsLongAs
}

public enum Fact
{
    [RegexPattern("enchanted artifact isn't")]
    Isnt
}

public enum FactValue
{
    [RegexPattern("a creature")]
    ACreature
}


