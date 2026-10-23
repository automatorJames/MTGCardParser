namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Elenda, Saint of Dusk, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Elenda, Saint of Dusk</exampledoc>
[TokenizationOrder(-1)]
public class ElendaSaintOfDuskRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["elenda gets an additional +5/+5 as long as your life total is at least 10 greater than your starting life total"];
}
