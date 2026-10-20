namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What damage is dealt to: a targetable player or card, a player ("you"), every object of a kind ("each creature and each player", "each creature your opponents control", "each other creature"), each of several targets ("each of two target creatures"), or one named earlier - "that player", "that land's controller".</summary>
/// <exampledoc>Copper Tablet</exampledoc>
/// <examplecapture>that player</examplecapture>
[Dependent]
public class Recipient : GlyphOneOf
{
    public Target Target { get; set; }
    public PlayerReference PlayerReference { get; set; }
    public ThatCard ThatCard { get; set; }
    public WhichPlayer? Player { get; set; }
    public ManyOf<EachObject> EachObjects { get; set; }
    public EachObject EachObject { get; set; }
    public EachPermanents EachPermanents { get; set; }
    public EachOfTargets EachOfTargets { get; set; }
}
