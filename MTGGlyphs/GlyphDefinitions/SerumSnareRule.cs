namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Serum Snare, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Serum Snare</exampledoc>
[TokenizationOrder(-1)]
public class SerumSnareRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if that permanent had mana value 3 or less, proliferate"];
}
