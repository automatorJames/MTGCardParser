namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Tamanoa, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tamanoa</exampledoc>
[TokenizationOrder(-1)]
public class TamanoaRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a noncreature source you control deals damage, you gain that much life"];
}
