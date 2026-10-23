namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Fire Lord Zuko, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Fire Lord Zuko</exampledoc>
[TokenizationOrder(-1)]
public class FireLordZukoCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you cast a spell from exile and whenever a permanent you control enters from exile, put a +1/+1 counter on each creature you control"];
}
