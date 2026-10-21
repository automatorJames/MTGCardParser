namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Manifesting or cloaking, as its reminder defines it: "put it onto the battlefield face down as a 2/2 creature", "… as a 2/2 creature with ward {2}" - a predicate.</summary>
/// <exampledoc>Cloudform</exampledoc>
/// <examplecapture>put it onto the battlefield face down as a 2/2 creature</examplecapture>
[Dependent]
public class PutFaceDownAsCreature : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["put", Prop(Moved), "onto the battlefield face down as a", Prop(Stats), "creature", Prop(With)];

    public OneOf<It, Them, PermanentPhrase> Moved { get; set; }
    public PowerToughness Stats { get; set; }
    [Optional]
    public WithKeywordAbility With { get; set; }
}
