namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Casting without paying the mana cost, sometimes limited to a period said first: "without paying its mana cost", "this turn without paying their mana costs".</summary>
/// <exampledoc>Golos, Tireless Pilgrim</exampledoc>
/// <examplecapture>this turn without paying their mana costs</examplecapture>
[Dependent]
public class WithoutPayingManaCost : Glyph
{
    public override Nib[] Nibs => [Prop(Period), Pattern("without paying (its|their) mana costs?")];

    [Optional]
    public ThisPeriod Period { get; set; }
}
