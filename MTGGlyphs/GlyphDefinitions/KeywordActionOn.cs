namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A keyword action done to something: "goad target creature", "detain target creature an opponent controls", "suspect it", "manifest the top card of your library", "manifest a card" (in its own reminder).</summary>
/// <exampledoc>Azorius Arrester</exampledoc>
/// <examplecapture>detain target creature an opponent controls</examplecapture>
[Dependent]
public class KeywordActionOn : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Prop(Action), Prop(Object)];

    public Keyword Action { get; set; }
    public OneOf<PermanentPhrase, CountedTargets, CardActedOn> Object { get; set; }
}
