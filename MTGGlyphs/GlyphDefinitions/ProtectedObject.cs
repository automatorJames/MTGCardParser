namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One thing a prevention effect protects: a player or particular object (a <see cref="Recipient"/>), or a group of permanents ("other creatures you control").</summary>
/// <exampledoc>Inner Sanctum</exampledoc>
/// <examplecapture>creatures you control</examplecapture>
[Dependent]
public class ProtectedObject : GlyphOneOf
{
    public Recipient Recipient { get; set; }
    public OtherPermanents Others { get; set; }
    public Permanents Permanents { get; set; }
}
