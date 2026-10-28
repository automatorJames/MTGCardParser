namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Alchemist's Gambit, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Alchemist's Gambit</exampledoc>
[TokenizationOrder(-1)]
public class AlchemistsGambitRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["during that turn, damage can't be prevented"];
}
