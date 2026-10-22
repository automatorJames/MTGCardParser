namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// "that creature", "those artifacts", "the creature", "the other creature", "that creature or planeswalker": refers back to a card type - a <see cref="CardType"/> marked [Referent] earlier
/// in the line, such as the creatures chosen in "choose any number of tapped blue creatures they control", or the creature met in combat ("blocks or becomes blocked by a creature, the other creature …") - or to a target of either of two types.
/// </summary>
/// <exampledoc>Magnetic Mountain</exampledoc>
/// <examplecapture>those creatures</examplecapture>
[Dependent]
public class ThatCard : BackReference<CardType>
{
    public override Nib[] Nibs => [Alt("that", "those", "the other", "the"), Plural(Prop(CardType)), Prop(OrType)];

    public CardType CardType { get; set; }
    [Optional]
    public OrCardType OrType { get; set; }
}
