namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One particular permanent: {this}, "this permanent" in reminder text, "it", a target, one named earlier ("that creature", "the exiled card"), or the enchanted one.</summary>
/// <exampledoc>Animate Dead</exampledoc>
/// <examplecapture>enchanted creature</examplecapture>
[Dependent]
public class PermanentPhrase : GlyphOneOf
{
    public This This { get; set; }
    public ThisCreature ThisCreature { get; set; }
    public It It { get; set; }
    public SpecificTarget SpecificTarget { get; set; }
    public ThatCard ThatCard { get; set; }
    public EnchantedPermanent EnchantedPermanent { get; set; }
    public ThatCardPhrase ThatCardPhrase { get; set; }
}
