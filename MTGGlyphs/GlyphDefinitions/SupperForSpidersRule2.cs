namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Supper for Spiders, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Supper for Spiders</exampledoc>
[TokenizationOrder(-1)]
public class SupperForSpidersRule2 : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["they are food artifacts with \"{2}, {t}, sacrifice this artifact: you gain 3 life"];
}
