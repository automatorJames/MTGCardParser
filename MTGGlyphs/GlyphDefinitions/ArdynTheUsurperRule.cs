namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ardyn, the Usurper, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ardyn, the Usurper</exampledoc>
[TokenizationOrder(-1)]
public class ArdynTheUsurperRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you exiled a card this way, create a token that's a copy of that card, except it's a 5/5 black demon"];
}
