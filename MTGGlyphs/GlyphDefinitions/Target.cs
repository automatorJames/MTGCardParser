namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A target: "any target", or "target" and what it targets.</summary>
/// <exampledoc>Aladdin's Ring</exampledoc>
/// <examplecapture>any target</examplecapture>
[Dependent]
public class Target : GlyphOneOf
{
    public AnyTarget AnyTarget { get; set; }
    public SpecificTarget SpecificTarget { get; set; }
}
