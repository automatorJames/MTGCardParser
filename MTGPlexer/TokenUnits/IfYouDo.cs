namespace MTGPlexer.TokenUnits;

//[IsolateForTesting]
public class IfYouDo : TokenUnit
{
    protected override string[] Snippets => ["if you do,", nameof(Outcome)];

    public DynamicCapture<TokenUnit> Outcome { get; set; }
}

//[IsolateForTesting]
public class EatSomething : TokenUnit
{
    protected override string[] Snippets => ["eat a", nameof(SomethingToEat)];
    public SomethingToEat SomethingToEat { get; set; }
}

//[IsolateForTesting]
//public class ATypeOfHobo : TokenUnit
//{
//    protected override string[] Snippets => [nameof(SomethingInsulting), "hobo is your", nameof(SomethingYourIs)];
//    public SomethingInsulting SomethingInsulting { get; set; }
//    public SomethingYourIs SomethingYourIs { get; set; }
//}

public enum SomethingToEat
{
    Apple,
    Dick,
    Taco
}

public enum SomethingInsulting
{
    Smelly,
    Dirty
}

public enum SomethingYourIs
{
    Bollocksis,
    Nameseses
}