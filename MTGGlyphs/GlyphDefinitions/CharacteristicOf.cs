namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A characteristic of one object, as an amount: "its power", "that creature's toughness", "the sacrificed artifact's mana value", "{this}'s power".</summary>
/// <exampledoc>Swords to Plowshares</exampledoc>
/// <examplecapture>its power</examplecapture>
[Dependent]
public class CharacteristicOf : Glyph
{
    public override Nib[] Nibs => [Prop(Object), Prop(Characteristic)];

    public OneOf<Its, ObjectPossessive> Object { get; set; }
    public Characteristic Characteristic { get; set; }
}
