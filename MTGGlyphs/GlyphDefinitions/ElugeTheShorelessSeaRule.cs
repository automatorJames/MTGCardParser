namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Eluge, the Shoreless Sea, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Eluge, the Shoreless Sea</exampledoc>
/// <examplecapture>the first instant or sorcery spell you cast each turn costs {u} (or {1}) less to cast for each land you control with a flood counter on it</examplecapture>
[TokenizationOrder(-1)]
public class ElugeTheShorelessSeaRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the first instant or sorcery spell you cast each turn costs {u} (or {1}) less to cast for each land you control with a flood counter on it"];
}
