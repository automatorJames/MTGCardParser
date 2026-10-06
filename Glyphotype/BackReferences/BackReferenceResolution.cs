namespace Glyphotype.BackReferences;

/// <summary>How a <see cref="BackReference"/> got - or didn't get - its <see cref="BackReference.Antecedent"/>.</summary>
public enum BackReferenceResolutionKind
{
    /// <summary>Nothing before it in its line agreed with it.</summary>
    Unresolved,

    /// <summary>Bound to the most recent agreeing referent before it in its line.</summary>
    Searched,

    /// <summary>Bound by its Glyph, with <see cref="RefersToAttribute"/>.</summary>
    Declared,
}

/// <summary>One <see cref="BackReference"/> on a line, where it was captured, and what it was resolved to (see <see cref="BackReferenceResolver"/>).</summary>
public sealed record BackReferenceResolution(BackReference BackReference, CaptureTrace Trace, BackReferenceResolutionKind Kind)
{
    /// <summary>What <see cref="BackReference"/> refers to, or null when <see cref="Kind"/> is <see cref="BackReferenceResolutionKind.Unresolved"/>.</summary>
    public ReferentCapture Antecedent => BackReference.Antecedent;

    public bool IsResolved => Kind != BackReferenceResolutionKind.Unresolved;
}
