namespace Glyphotype.GlyphPrimitives;

/// <summary>
/// The period that separates one clause from the next, as a first-class token rather than leftover text.
/// <para>
/// Synthesized directly by the <see cref="Tokenizers.Tokenizer"/> (like <see cref="UnmatchedString"/>,
/// and for the same reason) instead of competing as a top-level type: a clause break sits *between*
/// segments, never at a segment's start, so under the whole-segment rule it could never be a candidate
/// anyway - and being registered would let some other type's regex shadow it. Making it a
/// <see cref="CaptureUnit"/> rather than a <see cref="Glyph"/> is also what keeps the registry's own type
/// discovery from picking it up as a top-level candidate.
/// </para>
/// <para>
/// Exists so that a period stops being reported as unmatched text. It isn't unmodeled - it's punctuation
/// the grammar treats as structure - so counting it as unmatched only pollutes the signal that says which
/// text still needs a Glyph written for it.
/// </para>
/// </summary>
[RegexBoundaryOptionAtrribute(BoundaryOption.None)]
public class ClauseBreak : CaptureUnit
{
    public ClauseBreak()
    {
    }

    public ClauseBreak(string sourceText, int index, int length)
    {
        var regexForLength = new Regex($".{{{length}}}", RegexOptions.Singleline);
        var match = regexForLength.Match(sourceText, index, length);

        // Should always match
        if (!match.Success)
            throw new Exception();

        CaptureContext = new(new ClauseBreakNode(null, new(typeof(ClauseBreak))), match, sourceText);
    }
}
