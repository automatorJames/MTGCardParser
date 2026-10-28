using Glyphotype.Interfaces;

namespace Glyphotype.Lowering;

/// <summary>One document as an <see cref="ILoweringEngine"/> lowered it: a tree per line, plus the whole as text (JSON, or whatever the engine writes) when it has one.</summary>
public sealed record LoweredDocument(IDocument Document, IReadOnlyList<LoweredLine> Lines, string Text = null)
{
    public bool IsFullyLowered => Lines.All(x => x.IsFullyLowered);
}

/// <summary>
/// One line of a document, lowered: the roots the engine made of it (one per top-level construction, in text order),
/// the kinds that stayed opaque, and how many references the engine couldn't bind.
/// </summary>
public sealed record LoweredLine(int LineIndex, string Text, IReadOnlyList<LoweredNode> Roots, IReadOnlyList<string> OpaqueKinds, int UnboundReferences = 0)
{
    public bool IsFullyLowered => OpaqueKinds.Count == 0;
}

/// <summary>How a node came out of lowering.</summary>
public enum LoweredNodeStatus
{
    /// <summary>A node the engine's representation has a shape for.</summary>
    Lowered,
    /// <summary>Text the engine has no shape for yet, held as it was. <see cref="LoweredNode.Note"/> names what produced it.</summary>
    Opaque,
    /// <summary>A terminal: a vocabulary member, a number, a word.</summary>
    Term,
    /// <summary>Something named earlier in the line, resolved to the node that introduced it (<see cref="LoweredNode.RefersTo"/>) or left unbound.</summary>
    Reference,
}

/// <summary>
/// One node of a lowered tree: its kind (the engine's name for the shape), the family that kind belongs to, how it
/// came out, its fields, and where in the line's text it came from, when the engine knows.
/// </summary>
/// <param name="Kind">The engine's name for the node's shape, e.g. "DealDamage".</param>
/// <param name="Family">Which of <see cref="ILoweringEngine.Families"/> the node belongs to.</param>
/// <param name="Text">What a terminal, opaque or reference node holds, as text; null for a lowered node, whose content is its fields.</param>
/// <param name="Note">A word about the node: the vocabulary a term is from, the glyph an opaque node came from, a reference's antecedent.</param>
/// <param name="Span">Where in the line's text the node came from, when known.</param>
/// <param name="Binding">The name this node is known by to later references ("#1"), when it introduces something they refer to.</param>
/// <param name="RefersTo">For a reference, the <paramref name="Binding"/> it was resolved to; null when unbound.</param>
public sealed record LoweredNode(
    string Kind,
    string Family,
    LoweredNodeStatus Status,
    IReadOnlyList<LoweredField> Fields,
    string Text = null,
    string Note = null,
    SourceSpan? Span = null,
    string Binding = null,
    string RefersTo = null)
{
    /// <summary>Whether no field holds a node, so the whole node reads on one line.</summary>
    public bool IsLeaf => Fields.All(x => x.Nodes is null);

    /// <summary>This node and every node under it, depth first.</summary>
    public IEnumerable<LoweredNode> Descendants()
    {
        yield return this;

        foreach (var field in Fields)
            if (field.Nodes is not null)
                foreach (var node in field.Nodes)
                    foreach (var descendant in node.Descendants())
                        yield return descendant;
    }
}

/// <summary>A field of a <see cref="LoweredNode"/>: either a scalar <paramref name="Value"/> (a number, a name, a flag) or one or more <paramref name="Nodes"/>.</summary>
public sealed record LoweredField(string Name, string Value = null, IReadOnlyList<LoweredNode> Nodes = null);

/// <summary>A span of a line's text: where a node came from.</summary>
public readonly record struct SourceSpan(int Index, int Length)
{
    public int End => Index + Length;
}

/// <summary>What lowering one document gives, counted: per line, whether it lowered fully and which kinds stayed opaque.</summary>
public sealed record LoweredSummary(IDocument Document, IReadOnlyList<LoweredLineSummary> Lines)
{
    public bool IsFullyLowered => Lines.All(x => x.IsFullyLowered);
}

public sealed record LoweredLineSummary(int LineIndex, IReadOnlyList<string> OpaqueKinds, IReadOnlyList<string> OpaqueTexts, int UnboundReferences = 0)
{
    public bool IsFullyLowered => OpaqueKinds.Count == 0;
}
