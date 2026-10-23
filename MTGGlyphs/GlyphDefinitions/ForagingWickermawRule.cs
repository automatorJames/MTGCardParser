namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Foraging Wickermaw, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Foraging Wickermaw</exampledoc>
[TokenizationOrder(-1)]
public class ForagingWickermawRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature becomes that color until end of turn"];
}
