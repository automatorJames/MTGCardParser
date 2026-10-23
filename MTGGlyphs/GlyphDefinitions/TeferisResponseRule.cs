namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Teferi's Response, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Teferi's Response</exampledoc>
[TokenizationOrder(-1)]
public class TeferisResponseRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["counter target spell or ability an opponent controls that targets a land you control"];
}
