namespace MTGGlyphs.GlyphDefinitions;

/// <summary>What an effect is dealt or put onto: "any target", "target creature", "that land's controller", "{this}".</summary>
[Dependent]
public class Recipient : GlyphOneOf
{
    public AnyTarget AnyTarget { get; set; }
    public Target Target { get; set; }
    public ThatCardsController ThatCardsController { get; set; }
    public This This { get; set; }
}
