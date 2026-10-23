namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rally the Horde, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rally the Horde</exampledoc>
[TokenizationOrder(-1)]
public class RallyTheHordeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the last card exiled isn't a land card, repeat this process"];
}
