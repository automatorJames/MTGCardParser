namespace MTGGlyphs.GlyphDefinitions;

[Dependent]
public class Recipient : GlyphOneOf
{
    public TargetableEntity TargetableEntity { get; set; }
    public ThatCardsController ThatCardsController { get; set; }
}
