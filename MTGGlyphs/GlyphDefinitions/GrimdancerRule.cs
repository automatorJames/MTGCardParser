namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Grimdancer, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Grimdancer</exampledoc>
[TokenizationOrder(-1)]
public class GrimdancerRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature enters with your choice of two different counters on it from among menace, deathtouch, and lifelink"];
}
