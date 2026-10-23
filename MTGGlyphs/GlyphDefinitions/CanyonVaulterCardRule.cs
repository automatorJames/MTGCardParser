namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Canyon Vaulter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Canyon Vaulter</exampledoc>
[TokenizationOrder(-1)]
public class CanyonVaulterCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever this creature saddles a mount or crews a vehicle during your main phase, that mount or vehicle gains flying until end of turn"];
}
