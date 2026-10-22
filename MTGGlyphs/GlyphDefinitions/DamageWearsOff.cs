namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Ending the turn, its last step: "damage wears off, and "this turn" and "until end of turn" effects end" (older: "damage heals and …").</summary>
/// <exampledoc>Glorious End</exampledoc>
/// <examplecapture>damage wears off, and "this turn" and "until end of turn" effects end</examplecapture>
public class DamageWearsOff : Glyph
{
    public override Nib[] Nibs => [Alt("damage wears off, and", "damage heals and"), "\"this turn\" and \"until end of turn\" effects end"];
}
