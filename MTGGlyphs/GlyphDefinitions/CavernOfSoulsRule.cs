namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Cavern of Souls, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cavern of Souls</exampledoc>
[TokenizationOrder(-1)]
public class CavernOfSoulsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["spend this mana only to cast a creature spell of the chosen type, and that spell can't be countered"];
}
