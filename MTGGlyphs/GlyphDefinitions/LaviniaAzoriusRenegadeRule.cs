namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Lavinia, Azorius Renegade, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lavinia, Azorius Renegade</exampledoc>
[TokenizationOrder(-1)]
public class LaviniaAzoriusRenegadeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each opponent can't cast noncreature spells with mana value greater than the number of lands that player controls"];
}
