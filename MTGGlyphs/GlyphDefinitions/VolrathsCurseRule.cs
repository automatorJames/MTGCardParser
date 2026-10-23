namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Volrath's Curse, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Volrath's Curse</exampledoc>
[TokenizationOrder(-1)]
public class VolrathsCurseRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that creature's controller may sacrifice a permanent of their choice for that player to ignore this effect until end of turn"];
}
