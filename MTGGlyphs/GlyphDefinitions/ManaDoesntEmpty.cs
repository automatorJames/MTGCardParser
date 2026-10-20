namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Mana that lingers: "you don't lose this mana as steps and phases end" (usually after "until end of turn,").</summary>
/// <exampledoc>Sakura-Tribe Elder</exampledoc>
/// <examplecapture>you don't lose this mana as steps and phases end</examplecapture>
public class ManaDoesntEmpty : Glyph
{
    public override Nib[] Nibs => [Opt("until end of turn,"), "you don't lose this mana as steps and phases end"];
}
