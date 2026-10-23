namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Soul Immolation, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Soul Immolation</exampledoc>
[TokenizationOrder(-1)]
public class SoulImmolationRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["x can't be greater than the greatest toughness among creatures you control"];
}
