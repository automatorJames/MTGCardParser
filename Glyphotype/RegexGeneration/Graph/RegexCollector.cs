namespace Glyphotype.RegexGeneration.Graph;

/// <summary>
/// Accumulates the flat sequence of <see cref="RegexBrick"/>s produced while walking a <see cref="RegexNode"/>
/// graph, and compiles them into a <see cref="BuiltRegex"/> once the walk is complete.
/// </summary>
public class RegexCollector
{
    /// <summary>The bricks appended so far, in emission order.</summary>
    public List<RegexBrick> RegexBricks { get; } = [];

    /// <summary>The last regex character emitted so far, ignoring group open/close bookends (used to decide whether a joiner is needed next).</summary>
    public char LastChar =>
        RegexBricks.LastOrDefault(x => x is not RegexBrickGroupBookend)
        .Regex.LastOrDefault();

    /// <summary>Appends a brick to the sequence.</summary>
    public void Append(RegexBrick brick)
    {
        var thing = this.ToString();
        if (brick.Regex == "[ ]" || brick.Regex == " ") Debugger.Break();
        RegexBricks.Add(brick);
    }

    /// <summary>Compiles the accumulated bricks into a <see cref="BuiltRegex"/>.</summary>
    public BuiltRegex GetBuiltRegex() =>
        new(RegexBricks);

    public override string ToString() =>
        string.Join("", RegexBricks.Select(x => x.Regex));
}