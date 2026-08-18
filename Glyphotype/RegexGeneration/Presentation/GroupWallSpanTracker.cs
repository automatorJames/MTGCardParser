namespace Glyphotype.RegexGeneration.Presentation;

/// <summary>
/// Tracks the stack of left/right border-wall spans for currently-open group boxes, so each line's
/// comment can be sandwiched between the correct nested walls. Callers must <see cref="Push"/> right
/// after rendering a group's opening line, and <see cref="Pop"/> right before rendering its closing line.
/// </summary>
internal class GroupWallSpanTracker
{
    readonly Stack<SmartSpan> _leftWalls = [];
    List<SmartSpan> _mirroredRightWalls = [];

    /// <summary>Left-hand wall spans, outermost first, for every group box currently open.</summary>
    public IEnumerable<SmartSpan> LeftWalls => _leftWalls.Reverse();

    /// <summary>How many group boxes are currently open — the live nesting depth at this point in rendering.</summary>
    public int Depth => _leftWalls.Count;

    /// <summary>Right-hand wall spans (mirrored copies of the left walls), innermost first, for every group box currently open.</summary>
    public IEnumerable<SmartSpan> RightWalls => _mirroredRightWalls;

    /// <summary>Opens a new nested group box, adding its wall to both the left and mirrored-right stacks.</summary>
    public void Push(SmartSpan leftWallSpan)
    {
        _leftWalls.Push(leftWallSpan);
        _mirroredRightWalls = _leftWalls.Select(x => x.Mirror()).ToList();
    }

    /// <summary>Closes the innermost open group box.</summary>
    public void Pop()
    {
        _leftWalls.Pop();
        _mirroredRightWalls = _leftWalls.Select(x => x.Mirror()).ToList();
    }
}
