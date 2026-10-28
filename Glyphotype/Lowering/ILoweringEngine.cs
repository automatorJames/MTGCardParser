using Glyphotype.GlyphAnalysisDTOs.SpanAnalysis;

namespace Glyphotype.Lowering;

/// <summary>
/// A grammar's optional second stage: lowering a tokenized document's glyphs into the representation some engine
/// consumes - the meaning of the text with its syntax taken away. Glyphotype knows nothing of what that
/// representation is; an engine describes its output as a tree of <see cref="LoweredNode"/>s, which the app shows
/// beside the text they came from, and summarizes it, which the app counts over the corpus.
/// <para>
/// Registered by the app that composes a grammar with its engine; an app with no engine shows no lowered view.
/// </para>
/// </summary>
public interface ILoweringEngine
{
    /// <summary>What the engine's output is called, as the app titles its view: "Rules IR", "Intents", …</summary>
    string Name { get; }

    /// <summary>
    /// The families a node can belong to - the kinds of slot the engine's representation has ("Effect", "Condition",
    /// …) - in a fixed order, so each keeps its color from one view to the next.
    /// </summary>
    IReadOnlyList<string> Families { get; }

    /// <summary>Lowers <paramref name="document"/>, as its grammar tokenized it, to the tree the app shows.</summary>
    LoweredDocument Lower(ProcessedDocument document);

    /// <summary>Just the counts <see cref="Lower"/> would give, for every document of a corpus at once: what lowered fully, and what stayed opaque.</summary>
    LoweredSummary Summarize(ProcessedDocument document);
}
