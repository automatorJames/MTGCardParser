namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Cunning Nightbonder, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cunning Nightbonder</exampledoc>
[TokenizationOrder(-1)]
public class CunningNightbonderRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spells with flash you cast cost {1} less to cast and can't be countered"];
}
