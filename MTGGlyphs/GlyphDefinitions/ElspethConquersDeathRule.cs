namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Elspeth Conquers Death, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Elspeth Conquers Death</exampledoc>
[TokenizationOrder(-1)]
public class ElspethConquersDeathRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["ii — noncreature spells your opponents cast cost {2} more to cast until your next turn"];
}
