namespace MTGPlexer.TokenUnits;

[NoSpaces]
[IsolateForTesting]
public class HasAbility : TokenUnit
{
    public HasAbility() : base("has \"", nameof(Ability) ,"\"") { }

    [RegexPattern("[^\"]+")]
    public DynamicCapture<TokenUnit> Ability { get; set; }
}

[IsolateForTesting]
public class VesuvanDoppelganger : TokenUnit
{
    [RegexPattern(@"at the beginning of your upkeep, you may have this creature become a copy of target creature, except it doesn't copy that creature's color and it has this ability\.")]
    public PlaceholderCapture TheFeckingText { get; set; }
}

[IsolateForTesting]
public class OneOfTest : TokenUnitOneOf
{
    public Alphabet Alphabet { get; set; }
    public MockThing MockThing { get; set; }
}

[IsolateForTesting]
public class OneOfTestWrapper : TokenUnitOneOf
{
    public MockThing2Wrapper MockThing2Wrapper { get; set; }
    public ActionWrapper ActionWrapper { get; set; }
}

[IsolateForTesting]
public class MockThing2Wrapper : TokenUnit
{
    public MockThing2 MockThing2 { get; set; }
}

[IsolateForTesting]
public class ActionWrapper : TokenUnit
{
    public MockAction MockAction { get; set; }
}


public enum MockThing
{
    Battlefield,
    Animal
}

public enum MockThing2
{
    Upkeep,
    Downkeep
}

public enum MockAction
{
    Enter,
    Proceed
}


