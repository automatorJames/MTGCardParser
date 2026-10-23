namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Maelstrom Muse, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Maelstrom Muse</exampledoc>
/// <examplecapture>the next instant or sorcery spell you cast this turn costs {x} less to cast, where x is this creature's power as this ability resolves</examplecapture>
[Dependent]
public class MaelstromMuseEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the next instant or sorcery spell you cast this turn costs {x} less to cast, where x is this creature's power as this ability resolves"];
}
