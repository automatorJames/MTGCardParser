namespace MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines;

public class OptionalTerminalPeriod : RegexElement
{
    public OptionalTerminalPeriod()
        : base([], @"(\.)?", comment: "optional terminal period")
    {
    }
}