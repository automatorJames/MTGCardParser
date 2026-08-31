namespace Glyphotype.RegexGeneration.Graph;

/// <summary>
/// Accumulates the flat sequence of <see cref="RegexBrick"/>s produced while walking a <see cref="RegexNode"/>
/// graph, and compiles them into a <see cref="BuiltRegex"/> once the walk is complete.
/// </summary>
public class RegexCollector
{
    /// <summary>The bricks appended so far, in emission order.</summary>
    public List<RegexBrick> RegexBricks { get; } = [];

    /// <summary>
    /// A trailing space - raw (e.g. from an unescaped <see cref="RegexPatternAttribute"/> pattern) or written
    /// as <see cref="BuiltRegex.EscapedSpace"/> - optionally sitting just inside a closing group that may
    /// itself be quantified. That last case is what catches an optional nib like <c>(an[ ])?</c> or
    /// <c>((in|from)[ ])*</c>, whose space is only emitted when the optional actually matches.
    /// </summary>
    static readonly Regex _trailingSpacePattern = new(@"(?: |\[ \])\)?[?*+]?$", RegexOptions.Compiled);

    /// <summary>
    /// Whether the regex emitted so far already accounts for the separation a joiner would add, so appending
    /// one would double it up. True for a plain trailing space, and also for a trailing *optional* group that
    /// ends in a space (see <see cref="_trailingSpacePattern"/>) - there the space only renders when the
    /// optional matches, which is exactly the case that needs it: when the optional matches nothing, the
    /// unconditional joiner that already preceded it is what separates its neighbours. Adding another joiner
    /// after it would space the matched case twice. (This reads the last emitted brick, so it assumes that
    /// preceding joiner was in fact emitted - true whenever the optional isn't the first thing in its group,
    /// which is the shape every case in this codebase takes.) Group open/close bookends are ignored, since
    /// they contribute no matchable text of their own.
    /// </summary>
    public bool AlreadySeparated =>
        RegexBricks.LastOrDefault(x => x is not RegexBrickGroupBookend)?.Regex is string regex
        && _trailingSpacePattern.IsMatch(regex);

    /// <summary>Appends a brick to the sequence.</summary>
    public void Append(RegexBrick brick) =>
        RegexBricks.Add(brick);

    /// <summary>Compiles the accumulated bricks into a <see cref="BuiltRegex"/>.</summary>
    public BuiltRegex GetBuiltRegex() =>
        new(RegexBricks);

    public override string ToString() =>
        string.Join("", RegexBricks.Select(x => x.Regex));
}