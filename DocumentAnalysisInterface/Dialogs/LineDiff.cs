namespace DocumentAnalysisInterface.Dialogs;

/// <summary>A line-wise diff of two texts, in the order a unified diff lists it: removed lines before the added lines that replace them.</summary>
public static class LineDiff
{
    public enum LineKind { Unchanged, Removed, Added }

    /// <summary>One line of a diff, with its number on each side (null on the side it isn't on).</summary>
    public sealed record Line(LineKind Kind, int? OldNumber, int? NewNumber, string Text);

    /// <summary>The lines of <paramref name="before"/> and <paramref name="after"/>, aligned by their longest common subsequence.</summary>
    public static IReadOnlyList<Line> Compare(string before, string after)
    {
        var old = Split(before);
        var @new = Split(after);

        // common[i, j]: the length of the longest common subsequence of old[i..] and new[j..].
        var common = new int[old.Length + 1, @new.Length + 1];

        for (int i = old.Length - 1; i >= 0; i--)
            for (int j = @new.Length - 1; j >= 0; j--)
                common[i, j] = old[i] == @new[j] ? common[i + 1, j + 1] + 1 : Math.Max(common[i + 1, j], common[i, j + 1]);

        List<Line> lines = [];
        int oldIndex = 0, newIndex = 0;

        while (oldIndex < old.Length || newIndex < @new.Length)
        {
            if (oldIndex < old.Length && newIndex < @new.Length && old[oldIndex] == @new[newIndex])
            {
                lines.Add(new(LineKind.Unchanged, oldIndex + 1, newIndex + 1, old[oldIndex]));
                oldIndex++;
                newIndex++;
            }
            else if (newIndex == @new.Length || (oldIndex < old.Length && common[oldIndex + 1, newIndex] >= common[oldIndex, newIndex + 1]))
            {
                lines.Add(new(LineKind.Removed, oldIndex + 1, null, old[oldIndex]));
                oldIndex++;
            }
            else
            {
                lines.Add(new(LineKind.Added, null, newIndex + 1, @new[newIndex]));
                newIndex++;
            }
        }

        return lines;
    }

    static string[] Split(string text) =>
        string.IsNullOrEmpty(text) ? [] : text.Replace("\r\n", "\n").TrimEnd('\n').Split('\n');
}
