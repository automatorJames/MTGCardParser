namespace MTGPlexer.TokenAnalysisDTOs.TypeExpressions;

public record RegexRoot(List<RegexFragment> Children) : RegexGroupFragment(RegexGroupType.Root, "", "", Children)
{
    public List<string> CaptureGroupNames { get; private set; } = GetRecursiveCaptureGroupNames(Children);

    static List<string> GetRecursiveCaptureGroupNames(List<RegexFragment> children, List<string> result = null)
    {
        result ??= [];

        foreach (var child in children.OfType<RegexGroupFragment>())
        {
            if (child.Name != null)
                result.Add(child.Name);

            if (child.Children?.Any() ?? false)
                RegexRoot.GetRecursiveCaptureGroupNames(child.Children, result);
        }

        return result;
    }
}

