namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"of the first equip ability you activate each turn": an alternative cost limited to the first activation of a keyword ability each turn.</summary>
/// <exampledoc>Bruenor Battlehammer</exampledoc>
/// <examplecapture>of the first equip ability you activate each turn</examplecapture>
[Dependent]
public class FirstAbilityEachTurn : Glyph
{
    public override Nib[] Nibs => ["of the first", Prop(Keyword), "ability you activate each turn"];

    public Keyword Keyword { get; set; }
}
