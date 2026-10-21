namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A rule about a modal spell's choices: "you may choose the same mode more than once".</summary>
/// <exampledoc>Planewide Celebration</exampledoc>
/// <examplecapture>you may choose the same mode more than once</examplecapture>
public class SameModeMoreThanOnce : Glyph
{
    public override Nib[] Nibs => ["you may choose the same mode more than once"];
}
