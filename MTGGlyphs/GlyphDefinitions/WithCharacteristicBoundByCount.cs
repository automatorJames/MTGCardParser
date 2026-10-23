namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"with power less than or equal to the number of creatures you control", "with mana value less than or equal to the number of lands you control", "with mana value less than or equal to the number of rust counters on it", "with mana value less than or equal to the number of cards in its controller's graveyard", "with mana value less than or equal to the amount of mana spent to cast this creature", "with mana value less than or equal to that damage": a characteristic bounded by a count, the mana spent on a spell, or an amount of damage.</summary>
/// <exampledoc>Beguiler of Wills</exampledoc>
/// <examplecapture>with power less than or equal to the number of creatures you control</examplecapture>
[Dependent]
public class WithCharacteristicBoundByCount : Glyph
{
    public override Nib[] Nibs => ["with", Prop(Characteristic), Prop(Comparison), "than or equal to", Prop(Count)];

    public Characteristic Characteristic { get; set; }
    public Comparison Comparison { get; set; }
    public OneOf<ControlledKindCount, CountersOnObjectCount, OneOf<CardsInZoneCount, ManaSpentAmount, OneOf<DamageReference?, Quantity?>>> Count { get; set; }
}
