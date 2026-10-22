namespace MTGGlyphs.GlyphDefinitions;

/// <summary>One kind of card a player may play or cast, with its verb unless it shares the one before: "play lands", "cast spells", "cast creature spells", "(cast artifact spells and) colorless spells".</summary>
/// <exampledoc>Future Sight</exampledoc>
/// <examplecapture>play lands</examplecapture>
[Dependent]
public class PlayPermission : Glyph
{
    public override Nib[] Nibs => [Prop(Verb), Prop(What)];

    [Optional]
    public PlayVerb? Verb { get; set; }
    public OneOf<OneOf<ThisCreature, ThatCard>, PlayableCards, PlayableCardType> What { get; set; }
}
