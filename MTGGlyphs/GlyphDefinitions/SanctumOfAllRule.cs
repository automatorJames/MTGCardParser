namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sanctum of All, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sanctum of All</exampledoc>
[TokenizationOrder(-1)]
public class SanctumOfAllRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a triggered ability of another shrine you control triggers while you control six or more shrines, that ability triggers an additional time"];
}
