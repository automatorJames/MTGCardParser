namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Codie, Ravenous Codex, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Codie, Ravenous Codex</exampledoc>
[Dependent]
public class CodieRavenousCodexEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["copy it"];
}
