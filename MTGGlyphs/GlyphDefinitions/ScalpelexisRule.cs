namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Scalpelexis, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Scalpelexis</exampledoc>
[TokenizationOrder(-1)]
public class ScalpelexisRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if two or more of those cards have the same name, repeat this process"];
}
