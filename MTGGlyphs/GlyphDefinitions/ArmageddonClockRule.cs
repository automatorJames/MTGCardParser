namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Armageddon Clock, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Armageddon Clock</exampledoc>
[TokenizationOrder(-1)]
public class ArmageddonClockRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["any player may activate this ability but only during any upkeep step"];
}
