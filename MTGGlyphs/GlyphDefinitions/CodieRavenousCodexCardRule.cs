namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Codie, Ravenous Codex, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Codie, Ravenous Codex</exampledoc>
[TokenizationOrder(-1)]
public class CodieRavenousCodexCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you cast a prepared spell, copy it"];
}
