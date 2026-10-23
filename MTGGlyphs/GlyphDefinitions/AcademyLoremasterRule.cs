namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Academy Loremaster, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Academy Loremaster</exampledoc>
[TokenizationOrder(-1)]
public class AcademyLoremasterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if they do, spells they cast this turn cost {2} more to cast"];
}
