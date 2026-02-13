namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

/// <summary>
/// Manages the construction of a logical sequence of regular expression elements. Acts as the single interface to translate RegexSegmentBase
/// components into properly-concatenated RegexElements, and ultimately composed Regex patterns. 
/// </summary>
public class RegexCollector
{
    public List<RegexBrick> RegexBricks { get; } = [];

    public void Append(RegexBrick brick) =>
        RegexBricks.Add(brick);

    public void AppendJoined(List<RegexNode> nodes, RegexBrick joiner)
    {
        for (int i = 0; i < RegexBricks.Count; i++)
        {
            nodes[i].AppendRegexBricks(this);

            if (i < RegexBricks.Count - 1)
                Append(joiner);
        }
    }

    ///// <summary>
    ///// Generates a fully formatted, commented, and colorized list of regex lines.
    ///// </summary>
    ///// <param name="synonymData">Optional data about captured synonyms to enrich the comments.</param>
    ///// <returns>A list of formatted regex lines.</returns>
    //public List<RegexFormattedLine> GetFormattedLines(List<PropPathSynonymSetContainer> synonymData = null)
    //{
    //    //var finalizedElements = _joiner.RegexElements.ToList();
    //    //AddBoundaryLines(finalizedElements);
    //    //var formatter = new RegexFormatter(finalizedElements, synonymData);
    //    //return formatter.Format();
    //
    //    return default;
    //}

    /// <summary>
    /// Generates a minified, single-line regex string.
    /// </summary>
    /// <returns>The complete regex as a single string.</returns>
    public string GetMinified()
    {
        return string.Join("", RegexBricks.Select(x => x.Regex)).Replace("[ ]", " ");
    }

    public BuiltRegex GetBuiltRegex()
    {
        var regexString = GetMinified();
        Regex regex = new(regexString, RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        var lines = RegexBricks.Select(x => x.Regex.Replace("[ ]", " ")).ToList();

        return new(regexString, regex, lines);
    }

    public override string ToString() => GetMinified();
}