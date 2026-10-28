namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"that card is" / "the top card of your library is" / "the discarded card was": a card named earlier, the top card of a library, or a sacrificed or discarded object, as the subject of "is" or "was".</summary>
/// <exampledoc>Chaos Harlequin</exampledoc>
/// <examplecapture>that card is</examplecapture>
[Dependent]
public class ThatCardIs : Glyph
{
    public override Nib[] Nibs => [Prop(Card), Prop(Tense)];

    public OneOf<ThatCardPhrase, TopCardOfLibrary, HandledObject> Card { get; set; }
    public StateTense Tense { get; set; }
}
