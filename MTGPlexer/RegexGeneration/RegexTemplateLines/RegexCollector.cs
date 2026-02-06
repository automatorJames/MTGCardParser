namespace MTGPlexer.RegexGeneration.RegexTemplateLines;

/// <summary>
/// Manages the construction of a logical sequence of regular expression elements. Acts as the single interface to translate RegexSegmentBase
/// components into properly-concatenated RegexElements, and ultimately composed Regex patterns. 
/// </summary>
public class RegexCollector
{
    public List<string> RegexLines { get; } = [];

    public void Append(string regexElement) =>
        RegexLines.Add(regexElement);

    /// <summary>
    /// Generates a fully formatted, commented, and colorized list of regex lines.
    /// </summary>
    /// <param name="synonymData">Optional data about captured synonyms to enrich the comments.</param>
    /// <returns>A list of formatted regex lines.</returns>
    public List<RegexFormattedLine> GetFormattedLines(List<PropPathSynonymSetContainer> synonymData = null)
    {
        var finalizedElements = _joiner.RegexElements.ToList();
        AddBoundaryLines(finalizedElements);
        var formatter = new RegexFormatter(finalizedElements, synonymData);
        return formatter.Format();
    }

    /// <summary>
    /// Generates a minified, single-line regex string.
    /// </summary>
    /// <returns>The complete regex as a single string.</returns>
    public string GetMinified(bool addBoundaries = true)
    {
        if (!_joiner.RegexElements.Any())
            return "";

        var finalizedElements = _joiner.RegexElements.ToList();

        if (addBoundaries)
            AddBoundaryLines(finalizedElements);

        return string.Join("", finalizedElements.Select(x => x.Regex)).Replace("[ ]", " ");
    }

    /// <summary>
    /// Adds start and end boundary elements to a list of regex lines based on the builder's boundary option.
    /// </summary>
    /// <param name="lines">The list of elements to add boundaries to.</param>
    void AddBoundaryLines(List<RegexElement> lines)
    {
        if (_boundaryOption == BoundaryOption.None)
            return;

        RegexElement startBoundary = _boundaryOption switch
        {
            BoundaryOption.WholeWord => new NegativeLookbehindBoundary(),
            BoundaryOption.FullLine => new StartOfLineBoundary(),
            _ => null
        };

        RegexElement endBoundary = _boundaryOption switch
        {
            BoundaryOption.WholeWord => new NegativeLookaheadBoundary(),
            BoundaryOption.FullLine => new EndOfLineBoundary(),
            _ => null
        };

        if (startBoundary != null)
        {
            lines.Insert(0, startBoundary);
            lines.Insert(1, new BlankLine([]));
        }

        if (endBoundary != null)
        {
            lines.Add(new BlankLine([]));
            lines.Add(endBoundary);
        }
    }

    public BuiltRegex GetBuiltRegex()
    {
        var regexString = GetMinified();
        Regex regex = new(regexString, RegexOptions.Compiled | RegexOptions.ExplicitCapture);
        var lines = GetFormattedLines();

        return new(regexString, regex, lines);
    }

    public override string ToString() => GetMinified(addBoundaries: false);
}