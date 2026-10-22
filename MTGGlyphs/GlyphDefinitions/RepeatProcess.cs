namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"repeat this process", "repeat this process once", "you may repeat this process any number of times", "repeat this process six more times": doing the preceding instructions again, optionally a set number of times. A predicate, so a player can be its subject and it can follow "if you do," or "then".</summary>
/// <exampledoc>Primal Surge</exampledoc>
/// <examplecapture>repeat this process</examplecapture>
public class RepeatProcess : Glyph, IConjoinedPredicate, IPredicate
{
    public override Nib[] Nibs => [Pattern("repeats?"), "this process", Prop(Times)];

    [Optional]
    public OneOf<RepeatCount?, MoreTimes> Times { get; set; }
}
