namespace Glyphotype.GlyphPrimitives;

/// <summary>
/// The opening or closing delimiter of an enclosure (see <see cref="Enclosures"/>) no glyph matched whole: the "("
/// and ")" around reminder text the Tokenizer tokenized the inside of on its own. Like a <see cref="ClauseBreak"/>,
/// it's punctuation the grammar treats as structure, not unmatched text - synthesized by the
/// <see cref="Tokenizers.Tokenizer"/>, never a candidate type.
/// </summary>
public class EnclosureMark : CaptureUnit
{
    public EnclosureMark()
    {
    }

    static readonly EnclosureMarkNode _rootNode = CreateSharedRootNode(new EnclosureMarkNode(null, new(typeof(EnclosureMark))));

    public EnclosureMark(string sourceText, int index, int depth)
    {
        InitializeSpanContext(_rootNode, sourceText, index, 1);
        Depth = depth;
    }

    /// <summary>How many enclosures this delimiter sits inside, its own excluded: 0 for one in the line's own text.</summary>
    public int Depth { get; }
}
