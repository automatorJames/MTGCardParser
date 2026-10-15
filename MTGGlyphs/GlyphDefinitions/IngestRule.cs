namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Ingest's reminder text.</summary>
/// <exampledoc>Mist Intruder</exampledoc>
/// <examplecapture>whenever this creature deals combat damage to a player, that player exiles the top card of their library</examplecapture>
public class IngestRule : Glyph
{
    public override Nib[] Nibs => ["whenever this creature deals combat damage to a player, that player exiles the top card of their library"];
}
