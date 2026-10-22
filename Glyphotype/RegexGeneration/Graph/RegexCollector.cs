namespace Glyphotype.RegexGeneration.Graph;

/// <summary>
/// Accumulates the flat sequence of <see cref="RegexBrick"/>s produced while walking a <see cref="RegexNode"/>
/// graph, and compiles them into a <see cref="BuiltRegex"/> once the walk is complete.
/// </summary>
public class RegexCollector
{
    /// <summary>The bricks appended so far, in emission order - none, for a collector that doesn't <see cref="KeepsBricks"/>.</summary>
    public List<RegexBrick> RegexBricks { get; } = [];

    /// <summary>The regex of the last brick appended that isn't a group bookend - what <see cref="AlreadySeparated"/> reads.</summary>
    protected string LastContentRegex { get; private set; }

    /// <summary>
    /// Whether <paramref name="group"/> - a nested glyph's - is to be appended as a call to its type's own pattern (see
    /// <see cref="AppendCall"/>) rather than walked: a collector building a <see cref="Matching.MatchPlan"/> says so of
    /// a large one, and so never walks the inlined graph beneath it. Never, by default: the bricks are the whole pattern.
    /// </summary>
    public virtual bool Calls(NamedGroupNode group) => false;

    /// <summary>Appends <paramref name="group"/>'s content as a call to its type's pattern, in place of its bricks (see <see cref="Calls"/>).</summary>
    public virtual void AppendCall(NamedGroupNode group) =>
        throw new NotSupportedException("This collector walks every group");

    /// <summary>Notes that content ending in <paramref name="regex"/> was appended, as far as <see cref="AlreadySeparated"/> is concerned - what a call stands in for.</summary>
    protected void NoteContent(string regex) => LastContentRegex = regex;

    /// <summary>Whether appended bricks are kept in <see cref="RegexBricks"/>. A collector that only reads them as they come (see <see cref="Matching.MatchPlan.Collector"/>) says no: a large graph's bricks run to millions.</summary>
    protected virtual bool KeepsBricks => true;

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
        LastContentRegex is string regex
        && _trailingSpacePattern.IsMatch(regex);

    /// <summary>Appends a brick to the sequence.</summary>
    public virtual void Append(RegexBrick brick)
    {
        if (brick is not RegexBrickGroupBookend)
            LastContentRegex = brick.Regex;

        if (KeepsBricks)
            RegexBricks.Add(brick);
    }

    public override string ToString() =>
        string.Join("", RegexBricks.Select(x => x.Regex));
}