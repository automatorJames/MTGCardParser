namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Reckless Velocitaur, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Reckless Velocitaur</exampledoc>
[TokenizationOrder(-1)]
public class RecklessVelocitaurCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever this creature saddles a mount or crews a vehicle during your main phase, that mount or vehicle gets +2/+0 and gains trample until end of turn"];
}
