namespace MTGPlexer.TokenUnits;

[NoWordBoundary]
public class WhereVariableIs : TokenUnit
{
    public WhereVariableIs() : base(", where", nameof(VariableName), "is ") { }

    public VariableName VariableName { get; set; }
}