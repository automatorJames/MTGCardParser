namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An amount of life given as "equal to" something: "life equal to its power", "life equal to the number of creature cards in your graveyard", "life equal to the life lost this way", "life equal to your devotion to green".</summary>
/// <exampledoc>Swords to Plowshares</exampledoc>
/// <examplecapture>life equal to its power</examplecapture>
[Dependent]
public class LifeEqualTo : Glyph
{
    public override Nib[] Nibs => ["life equal to", Prop(Amount)];

    public Amount Amount { get; set; }
}
