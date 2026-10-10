namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// "that creature", "those artifacts", "the creature": refers back to a card type - a <see cref="CardType"/> marked [Referent] earlier
/// in the line, such as the creatures chosen in "choose any number of tapped blue creatures they control".
/// </summary>
/// <exampledoc>Magnetic Mountain</exampledoc>
/// <examplecapture>those creatures</examplecapture>
[Dependent]
public class ThatCard : BackReference<CardType>
{
    public override Nib[] Nibs => [Alt("that", "those", "the"), Plural(Prop(CardType))];

    public CardType CardType { get; set; }
}
