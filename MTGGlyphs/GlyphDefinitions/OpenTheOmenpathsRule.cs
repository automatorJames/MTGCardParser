namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Open the Omenpaths, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Open the Omenpaths</exampledoc>
[TokenizationOrder(-1)]
public class OpenTheOmenpathsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["• add two mana of any one color and two mana of any other color"];
}
