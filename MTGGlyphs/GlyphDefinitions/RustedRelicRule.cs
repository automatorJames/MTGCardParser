namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rusted Relic, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rusted Relic</exampledoc>
[TokenizationOrder(-1)]
public class RustedRelicRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["metalcraft — this artifact is a 5/5 golem artifact creature as long as you control three or more artifacts"];
}
