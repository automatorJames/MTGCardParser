namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"at end of combat" (or a delayed "at this turn's next end of combat"): the end of combat step of the current turn, as a trigger point.</summary>
/// <exampledoc>Clockwork Avian</exampledoc>
/// <examplecapture>at end of combat</examplecapture>
[Dependent]
public class AtEndOfCombat : Glyph
{
    public override Nib[] Nibs => ["at", Opt("this turn's next"), "end of combat"];
}
