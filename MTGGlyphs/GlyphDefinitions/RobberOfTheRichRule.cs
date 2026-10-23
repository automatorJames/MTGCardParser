namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Robber of the Rich, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Robber of the Rich</exampledoc>
/// <examplecapture>during any turn you attacked with a rogue, you may cast that card and you may spend mana as though it were mana of any color to cast that spell</examplecapture>
[TokenizationOrder(-1)]
public class RobberOfTheRichRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["during any turn you attacked with a rogue, you may cast that card and you may spend mana as though it were mana of any color to cast that spell"];
}
