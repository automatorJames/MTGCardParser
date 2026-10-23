namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One thing a prevention effect protects: a player or particular object (a <see cref="Recipient"/>), a group of permanents ("other creatures you control"), a counted group of targets ("up to two target creatures"), or targets sharing the prevention ("any number of targets, divided as you choose").</summary>
/// <exampledoc>Inner Sanctum</exampledoc>
/// <examplecapture>creatures you control</examplecapture>
[Dependent]
public class ProtectedObject : GlyphOneOf
{
    public DividedProtected Divided { get; set; }
    public PermanentGroup Group { get; set; }
    public Recipient Recipient { get; set; }
    public OtherPermanents Others { get; set; }
    public Permanents Permanents { get; set; }
}
