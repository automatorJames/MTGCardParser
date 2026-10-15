namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Clue's reminder text.</summary>
/// <exampledoc>Thraben Inspector</exampledoc>
/// <examplecapture>create a colorless clue artifact token with "{2}, sacrifice this artifact: draw a card</examplecapture>
public class ClueRule : Glyph
{
    public override Nib[] Nibs => ["create a colorless clue artifact token with \"{2}, sacrifice this artifact: draw a card"];
}
