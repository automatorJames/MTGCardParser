namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ultima, Origin of Oblivion, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ultima, Origin of Oblivion</exampledoc>
[TokenizationOrder(-1)]
public class UltimaOriginOfOblivionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for as long as that land has a blight counter on it, it loses all land types and abilities and has \"{t}: add {c}"];
}
