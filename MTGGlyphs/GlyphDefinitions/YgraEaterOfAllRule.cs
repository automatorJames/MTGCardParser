namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ygra, Eater of All, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ygra, Eater of All</exampledoc>
[TokenizationOrder(-1)]
public class YgraEaterOfAllRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["other creatures are food artifacts in addition to their other types and have \"{2}, {t}, sacrifice this permanent: you gain 3 life"];
}
