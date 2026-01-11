namespace MTGPlexer.RegexGeneration.RegexTemplateLines.BuilderLines;

public class SpaceLine : RegexElement
{
    public SpaceLine(Enclosure[] enclosures, bool isOptional = false)
        : base(enclosures, $"[ ]{(isOptional ? "?" : "")}", comment: $"{(isOptional ? "optional " : "")}connective space")
    {
    }
}