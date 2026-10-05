namespace MTGGlyphs.GlyphDefinitions;

[Dependent]
public class TargetableEntity : GlyphOneOf
{
    public PlayerIdentity? PlayerIdentity { get; set; }
    public CardOrCreatureType CardOrCreatureType { get; set; }
}
