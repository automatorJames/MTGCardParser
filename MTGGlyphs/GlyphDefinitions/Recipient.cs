namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What damage is dealt to: a targetable player or card, or one named earlier - "that player", "that land's controller".</summary>
/// <exampledoc>Copper Tablet</exampledoc>
/// <examplecapture>that player</examplecapture>
[Dependent]
public class Recipient : GlyphOneOf
{
    public TargetableEntity TargetableEntity { get; set; }
    public ThatCardsController ThatCardsController { get; set; }
    public ThatPlayer ThatPlayer { get; set; }
    public ThatCard ThatCard { get; set; }
}
