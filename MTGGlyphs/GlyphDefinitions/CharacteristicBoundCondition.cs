namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An object's power, toughness or mana value reaching a bound, as a condition: "this creature's power is 4 or greater", "that creature's power is 0 or less", "its mana value is 2 or less", "its toughness is less than or equal to the number of cards in your graveyard", or as it last was, "its mana value was 3 or less" (after it's destroyed or returned).</summary>
/// <exampledoc>Bloodshot Trainee</exampledoc>
/// <examplecapture>this creature's power is 4 or greater</examplecapture>
[Dependent]
public class CharacteristicBoundCondition : Glyph
{
    public override Nib[] Nibs => [Prop(Object), Prop(Characteristic), Prop(Tense), Prop(Bound)];

    public OneOf<Its, ObjectPossessive> Object { get; set; }
    public Characteristic Characteristic { get; set; }
    public StateTense Tense { get; set; }
    public OneOf<NumericBound, BoundByAmount> Bound { get; set; }
}
