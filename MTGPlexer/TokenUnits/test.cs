namespace MTGPlexer.TokenUnits;

public class Test : TokenUnit
{
    public Test() : base("graveyard", nameof(TestChild)) { }
    public TestChild TestChild { get; set; }

}

public class TestChild : TokenUnit
{
    public TestChild() : base("from the", nameof(TestChildChild)) { }

    public TestChildChild TestChildChild { get; set; }

}

public class TestChildChild : TokenUnit
{
    public TestChildChild() : base("battlefield") { }

}


