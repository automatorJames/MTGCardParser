namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Imaginary Threats, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Imaginary Threats</exampledoc>
[TokenizationOrder(-1)]
public class ImaginaryThreatsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["during that player's next untap step, creatures they control don't untap"];
}
