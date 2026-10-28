namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Cavernous Maw, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cavernous Maw</exampledoc>
[TokenizationOrder(-1)]
public class CavernousMawRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it's still a cave land"];
}
