namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The head of a modal spell or ability: "choose one —"; each mode follows on its own line (see <see cref="Mode"/>).</summary>
/// <exampledoc>Blue Elemental Blast</exampledoc>
/// <examplecapture>choose one —</examplecapture>
public class ChooseOne : Glyph
{
    public override Nib[] Nibs => ["choose one —"];
}
