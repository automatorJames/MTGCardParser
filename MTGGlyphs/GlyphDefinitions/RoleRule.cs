namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The role token's reminder text: only one role per controller on a creature.</summary>
/// <exampledoc>Charmed Clothier</exampledoc>
/// <examplecapture>if you control another role on it, put that one into the graveyard</examplecapture>
public class RoleRule : Glyph
{
    public override Nib[] Nibs => ["if you control another role on it, put that one into the graveyard"];
}
