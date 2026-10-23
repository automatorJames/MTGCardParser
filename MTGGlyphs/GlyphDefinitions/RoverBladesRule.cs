namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Rover Blades, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Rover Blades</exampledoc>
[TokenizationOrder(-1)]
public class RoverBladesRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["creatures can't be attached to other permanents"];
}
