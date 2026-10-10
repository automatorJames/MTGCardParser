namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A static ability setting an object's power and/or toughness to an amount: "{this}'s power and toughness are each equal to the number of swamps you control".</summary>
/// <exampledoc>Nightmare</exampledoc>
/// <examplecapture>{this}'s power and toughness are each equal to the number of swamps you control</examplecapture>
public class PowerToughnessEqual : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(Object), Prop(PowerAndOrToughness), Alt("are each", "is"), "equal to", Prop(Amount)];

    public OneOf<Its, ObjectPossessive> Object { get; set; }
    public PowerAndOrToughness PowerAndOrToughness { get; set; }
    public OneOf<NumberOf, CharacteristicOf> Amount { get; set; }
}
