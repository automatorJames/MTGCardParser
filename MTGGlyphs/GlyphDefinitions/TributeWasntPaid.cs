namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Condition on tribute: "tribute wasn't paid" (an opponent declined to put the counters on it).</summary>
/// <exampledoc>Nessian Demolok</exampledoc>
/// <examplecapture>tribute wasn't paid</examplecapture>
[Dependent]
public class TributeWasntPaid : Glyph
{
    public override Nib[] Nibs => ["tribute wasn't paid"];
}
