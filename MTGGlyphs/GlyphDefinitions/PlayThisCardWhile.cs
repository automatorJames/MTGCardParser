namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Casting or playing the card itself for as long as something holds: "cast this card for as long as it remains exiled". Its own glyph, since adding "this card" to what PlayCard casts reparses "you cast this spell" triggers.</summary>
/// <exampledoc>Rakish Revelers</exampledoc>
/// <examplecapture>cast this card for as long as it remains exiled</examplecapture>
[Dependent]
public class PlayThisCardWhile : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(Verb), Prop(Card), Prop(Period)];

    public PlayVerb Verb { get; set; }
    public OneOf<This, ThisCreature> Card { get; set; }
    public ForAsLongAs Period { get; set; }
}
