namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"is every creature type", "is all creature types": a permanent (or card) having every creature type at once, as changeling's reminder says ("this card is every creature type").</summary>
/// <exampledoc>Runed Stalactite</exampledoc>
/// <examplecapture>is every creature type</examplecapture>
[Dependent]
public class IsEveryCreatureType : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => ["is", Alt("every", "all"), Plural("creature type")];
}
