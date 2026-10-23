namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Trinisphere, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Trinisphere</exampledoc>
[TokenizationOrder(-1)]
public class Trinisphere3Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for example, a spell that would cost {1}{b} to cast costs {2}{b} to cast instead"];
}
