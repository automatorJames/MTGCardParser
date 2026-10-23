namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Abilities of one keyword: "equip abilities", "an equip ability".</summary>
/// <exampledoc>Ronin, Shadow Stalker</exampledoc>
/// <examplecapture>equip abilities</examplecapture>
[Dependent]
public class KeywordAbilities : Glyph
{
    public override Nib[] Nibs => [Opt("an"), Prop(Keyword), Alt("abilities", "ability")];

    public Keyword Keyword { get; set; }
}
