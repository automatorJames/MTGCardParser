namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Geralf, the Fleshwright, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Geralf, the Fleshwright</exampledoc>
[TokenizationOrder(-1)]
public class GeralfTheFleshwrightCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you cast a spell during your turn other than your first spell that turn, create a 2/2 blue and black zombie rogue creature token"];
}
