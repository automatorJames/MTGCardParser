namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Prismari, the Inspiration, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Prismari, the Inspiration</exampledoc>
[Dependent]
public class PrismariTheInspirationEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you cast an instant or sorcery spell, copy it for each spell cast before it this turn"];
}
