namespace MTGPlexer.RegexGeneration.RegexTemplateLines.Lines;

public class NamedGroupClose : EncloureBookend
{
    public NamedGroupClose(Enclosure[] enclosures, string name, GroupQuantifier? quantifier = null)
        : base(
            enclosures,
            $"){(quantifier.HasValue ? quantifier.Value.Description() : "")}",
            GetComment(name, quantifier)
        )
    {
    }

    static string GetComment(string name, GroupQuantifier? quantifier)
    {
        var quantifierPart = quantifier.HasValue ? $" {quantifier.Value.ToString().ToFriendlyCase()}" : "";
        return $"{name.ToFriendlyCase(TitleDisplayOption.Title)}{quantifierPart}";
    }

    public override string ToString() => base.ToString();
}