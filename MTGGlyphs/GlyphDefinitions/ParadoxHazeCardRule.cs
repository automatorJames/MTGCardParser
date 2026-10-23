namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Paradox Haze, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Paradox Haze</exampledoc>
[TokenizationOrder(-1)]
public class ParadoxHazeCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["at the beginning of enchanted player's first upkeep each turn, that player gets an additional upkeep step after this step"];
}
