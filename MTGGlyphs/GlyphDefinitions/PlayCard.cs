namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Playing or casting a card or spell: one named earlier ("play that card", "cast it without paying its mana cost", "play those cards this turn"), a copy ("cast the copy without paying its mana cost"), a target card in a zone ("cast target instant card from your graveyard"), or spells of a kind ("cast a spell with mana value 3 or less from your hand without paying its mana cost") - possibly saying where a named card is cast from ("cast that card from exile").</summary>
/// <exampledoc>Count on Luck</exampledoc>
/// <examplecapture>play that card this turn</examplecapture>
[Dependent]
public class PlayCard : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(Verb), Prop(Card), Prop(From), Prop(Free), Prop(Period)];

    public PlayVerb Verb { get; set; }
    public CastObject Card { get; set; }
    [Optional]
    public CastFrom? From { get; set; }
    [RegexPattern("without paying (its|their) mana costs?")]
    public bool Free { get; set; }
    [Optional]
    public ThisPeriod Period { get; set; }
}
