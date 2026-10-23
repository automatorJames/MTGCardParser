namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A characteristic of one object, as an amount: "its power", "their power" (each of several objects' own), "his/her power" (a legend's), "that creature's toughness", "the sacrificed artifact's mana value", "{this}'s power", "target creature's power".</summary>
/// <exampledoc>Swords to Plowshares</exampledoc>
/// <examplecapture>its power</examplecapture>
[Dependent]
public class CharacteristicOf : Glyph
{
    public override Nib[] Nibs => [Prop(Object), Prop(Characteristic)];

    public OneOf<OneOf<Its, Their, PersonalPossessive>, ObjectPossessive, TargetPossessive> Object { get; set; }
    public Characteristic Characteristic { get; set; }
}
