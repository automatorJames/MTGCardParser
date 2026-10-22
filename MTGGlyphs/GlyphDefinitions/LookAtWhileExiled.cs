namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Looking at a face-down exiled card for as long as it stays exiled: "look at that card for as long as it remains exiled", "look at those cards for as long as they remain exiled".</summary>
/// <exampledoc>Jester's Scepter</exampledoc>
/// <examplecapture>look at those cards for as long as they remain exiled</examplecapture>
[Dependent]
public class LookAtWhileExiled : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["look at", Prop(Card), Prop(Period)];

    public OneOf<ThatCardPhrase, It, Them> Card { get; set; }
    public ForAsLongAs Period { get; set; }
}
