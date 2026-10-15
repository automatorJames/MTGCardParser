namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Vanishing's last clause: "when the last is removed, sacrifice it" or cast variant for suspend.</summary>
/// <exampledoc>Rift Bolt</exampledoc>
/// <examplecapture>when the last is removed, cast it without paying its mana cost</examplecapture>
public class SuspendLastRemoved : Glyph
{
    public override Nib[] Nibs => ["when the last is removed, cast it without paying its mana cost"];
}
