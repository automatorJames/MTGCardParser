namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Alhammarret, High Arbiter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Alhammarret, High Arbiter</exampledoc>
[TokenizationOrder(-1)]
public class AlhammarretHighArbiterRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["your opponents can't cast spells with the chosen name (as long as this creature is on the battlefield)"];
}
