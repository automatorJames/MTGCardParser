namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ochre Jelly, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ochre Jelly</exampledoc>
[TokenizationOrder(-1)]
public class OchreJellyRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the token enters with half that many +1/+1 counters on it, rounded down"];
}
