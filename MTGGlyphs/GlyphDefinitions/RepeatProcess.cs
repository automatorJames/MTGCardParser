namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"repeat this process", "repeat this process once", "you may repeat this process any number of times", "repeat this process six more times", "then do it again", "repeat this process for artifacts and creatures", "do the same for flying, first strike, …": doing the preceding instructions again, optionally a set number of times or with other objects. A predicate, so a player can be its subject and it can follow "if you do," or "then".</summary>
/// <exampledoc>Primal Surge</exampledoc>
/// <examplecapture>repeat this process</examplecapture>
public class RepeatProcess : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Prop(Phrase), Prop(Times), Prop(ForEach)];

    public RepeatPhrase Phrase { get; set; }
    [Optional]
    public OneOf<RepeatCount?, MoreTimes> Times { get; set; }
    [Optional]
    public RepeatFor ForEach { get; set; }
}
