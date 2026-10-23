namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dust Animus, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dust Animus</exampledoc>
[TokenizationOrder(-1)]
public class DustAnimusRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you control five or more untapped lands, this creature enters with two +1/+1 counters and a lifelink counter on it"];
}
