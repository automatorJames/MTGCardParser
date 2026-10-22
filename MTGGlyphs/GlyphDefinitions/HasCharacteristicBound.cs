namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An object having a characteristic within a bound, as a condition: "it has mana value 3 or less", "that card has mana value 2 or less", "that creature has toughness 6 or greater".</summary>
/// <exampledoc>Fatal Push</exampledoc>
/// <examplecapture>it has mana value 2 or less</examplecapture>
[Dependent]
public class HasCharacteristicBound : Glyph
{
    public override Nib[] Nibs => [Prop(Object), "has", Prop(Characteristic), Prop(Bound)];

    public OneOf<It, ThatCardPhrase, PermanentPhrase> Object { get; set; }
    public Characteristic Characteristic { get; set; }
    public OneOf<NumericBound, BoundByAmount> Bound { get; set; }
}
