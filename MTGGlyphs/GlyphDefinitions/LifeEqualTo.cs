namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An amount of life given as "equal to" something: "life equal to its power".</summary>
/// <exampledoc>Swords to Plowshares</exampledoc>
/// <examplecapture>life equal to its power</examplecapture>
[Dependent]
public class LifeEqualTo : Glyph
{
    public override Nib[] Nibs => ["life equal to", Prop(Amount)];

    public CharacteristicOf Amount { get; set; }
}
