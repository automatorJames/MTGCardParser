namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Lutri, the Spellchaser, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lutri, the Spellchaser</exampledoc>
[TokenizationOrder(-1)]
public class LutriTheSpellchaserRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["companion — each nonland card in your starting deck has a different name"];
}
