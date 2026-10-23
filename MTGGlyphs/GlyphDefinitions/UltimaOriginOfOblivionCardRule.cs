namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ultima, Origin of Oblivion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ultima, Origin of Oblivion</exampledoc>
[TokenizationOrder(-1)]
public class UltimaOriginOfOblivionCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you tap a land for {c}, add an additional {c}"];
}
