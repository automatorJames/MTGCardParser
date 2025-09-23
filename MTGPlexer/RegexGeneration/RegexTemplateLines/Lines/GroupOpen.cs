namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public record GroupOpen
(
    Enclosure[] Enclosures
) 
    : EncloureBookend
    (
        Enclosures: Enclosures,
        Regex: $"("
    )
{
    public override string ToString() => base.ToString();
}