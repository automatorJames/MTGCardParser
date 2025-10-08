namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public interface IMatchableAlternateEnum : IMatchableAlternate
{
    public Type EnumType { get; }
    public int EnumMemberCount { get; }
}
