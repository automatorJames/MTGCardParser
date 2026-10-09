namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One particular permanent: {this}, "it", a target, one named earlier ("that creature"), or the enchanted one.</summary>
/// <exampledoc>Animate Dead</exampledoc>
/// <examplecapture>enchanted creature</examplecapture>
[Dependent]
public class PermanentPhrase : GlyphOneOf
{
    public This This { get; set; }
    public It It { get; set; }
    public SpecificTarget SpecificTarget { get; set; }
    public ThatCard ThatCard { get; set; }
    public EnchantedPermanent EnchantedPermanent { get; set; }
}
