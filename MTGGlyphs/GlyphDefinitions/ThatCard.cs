namespace MTGGlyphs.GlyphDefinitions;

/// <summary>
/// "that creature", "those artifacts": refers back to a card type - a <see cref="CardType"/> marked [Referent] earlier
/// in the line, such as the creatures chosen in "choose any number of tapped blue creatures they control".
/// </summary>
[Dependent]
public class ThatCard : BackReference<CardType>
{
    public override Nib[] Nibs => [Alt("that", "those"), Plural(Prop(CardType))];

    public CardType CardType { get; set; }
}
