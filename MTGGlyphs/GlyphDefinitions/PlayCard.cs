namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Playing or casting a card named earlier, usually one exiled by an impulse effect: "play that card", "cast it without paying its mana cost", "play those cards this turn".</summary>
/// <exampledoc>Count on Luck</exampledoc>
/// <examplecapture>play that card this turn</examplecapture>
[Dependent]
public class PlayCard : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(Verb), Prop(Card), Prop(Free), Prop(Period)];

    public PlayVerb Verb { get; set; }
    public OneOf<ThatCardPhrase, It, Them> Card { get; set; }
    [RegexPattern("without paying (its|their) mana costs?")]
    public bool Free { get; set; }
    [Optional]
    public ThisPeriod Period { get; set; }
}
