namespace MTGGlyphs.GlyphDefinitions;

/// <summary>The daybound-era werewolf condition: "no spells were cast last turn".</summary>
/// <exampledoc>Afflicted Deserter // Werewolf Ransacker</exampledoc>
/// <examplecapture>no spells were cast last turn</examplecapture>
[Dependent]
public class NoSpellsCastLastTurn : Glyph
{
    public override Nib[] Nibs => ["no spells were cast last turn"];
}
