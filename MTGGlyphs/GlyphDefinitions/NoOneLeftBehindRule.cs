namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of No One Left Behind, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>No One Left Behind</exampledoc>
[TokenizationOrder(-1)]
public class NoOneLeftBehindRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this spell costs {3} less to cast if it targets a creature card with mana value 3 or less"];
}
