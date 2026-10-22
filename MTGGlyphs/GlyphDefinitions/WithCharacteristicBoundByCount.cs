namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"with power less than or equal to the number of creatures you control", "with mana value less than or equal to the number of lands you control", "with mana value less than or equal to the number of rust counters on it", "with mana value less than or equal to the number of cards in its controller's graveyard", "with mana value less than or equal to the amount of mana spent to cast this creature": a characteristic bounded by a count or the mana spent on a spell.</summary>
/// <exampledoc>Beguiler of Wills</exampledoc>
/// <examplecapture>with power less than or equal to the number of creatures you control</examplecapture>
[Dependent]
public class WithCharacteristicBoundByCount : Glyph
{
    public override Nib[] Nibs => ["with", Prop(Characteristic), Prop(Comparison), "than or equal to", Prop(Count)];

    public Characteristic Characteristic { get; set; }
    public Comparison Comparison { get; set; }
    public OneOf<ControlledKindCount, CountersOnObjectCount, OneOf<CardsInZoneCount, ManaSpentAmount>> Count { get; set; }
}
