namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Elspeth Conquers Death, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Elspeth Conquers Death</exampledoc>
[TokenizationOrder(-1)]
public class ElspethConquersDeathCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["put a +1/+1 counter or a loyalty counter on it"];
}
