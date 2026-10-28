namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Damping Engine, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Damping Engine</exampledoc>
[TokenizationOrder(-1)]
public class DampingEngineRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player may sacrifice a permanent of their choice for that player to ignore this effect until end of turn"];
}
