namespace MTGGlyphs.GlyphDefinitions;

[Dependent]
public class Recipient : GlyphOneOf
{
    public TargetableEntity TargetableEntity { get; set; }
    public ThatCardsController ThatCardsController { get; set; }
    public ThatPlayer ThatPlayer { get; set; }
    public ThatCard ThatCard { get; set; }
}
