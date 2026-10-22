namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A choice made as part of an effect: "choose an opponent", "choose a color and an opponent", "choose a nonland card name", "choose an artifact card name", "an opponent chooses a creature type", "choose another creature you control", "each player chooses three permanents they control". Also a predicate, so "you may choose a color" resolves.</summary>
/// <exampledoc>Jihad</exampledoc>
/// <examplecapture>choose a color and an opponent</examplecapture>
public class Choose : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Alt("choose", "chooses"), Prop(Choices)];

    public OneOf<ManyOf<Choice>, Choice?, OneOf<CountedTargets, PermanentPhrase, OneOf<IndefinitePermanent, QualifiedCardName, CountedPermanents>>> Choices { get; set; }
}
