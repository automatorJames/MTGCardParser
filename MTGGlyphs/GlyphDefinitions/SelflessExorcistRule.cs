namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Selfless Exorcist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Selfless Exorcist</exampledoc>
[TokenizationOrder(-1)]
public class SelflessExorcistRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that card deals damage equal to its power to this creature"];
}
