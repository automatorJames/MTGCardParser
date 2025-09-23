namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public record NamedGroupClose
(
    Enclosure[] Enclosures,
    string Name, 
    GroupQuantifier? Quantifier = null
) 
    : RegexTemplateLine
    (
        Enclosures: Enclosures,
        Regex: $"){(Quantifier.HasValue ?  Quantifier.Value.Description() : "")}", 
        Comment: GetComment(Name, Quantifier)
    )
{
    static string GetComment(string name, GroupQuantifier? quantifier)
    {
        var quantifierPart = quantifier.HasValue ? $" {quantifier.Value.ToString().ToFriendlyCase()}" : "";
        return $"{name.ToFriendlyCase(TitleDisplayOption.Title)}{quantifierPart}";
    }

    public override string ToString() => base.ToString();
}
