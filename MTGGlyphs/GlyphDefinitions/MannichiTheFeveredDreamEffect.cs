namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mannichi, the Fevered Dream, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mannichi, the Fevered Dream</exampledoc>
[Dependent]
public class MannichiTheFeveredDreamEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["switch each creature's power and toughness until end of turn"];
}
