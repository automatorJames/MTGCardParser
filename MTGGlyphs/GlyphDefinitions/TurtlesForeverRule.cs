namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Turtles Forever, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Turtles Forever</exampledoc>
[TokenizationOrder(-1)]
public class TurtlesForeverRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["search your library and/or outside the game for exactly four legendary creature cards you own with different names, then reveal those cards"];
}
