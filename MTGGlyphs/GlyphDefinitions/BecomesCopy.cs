namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A permanent taking on another object's copiable values: "enter as a copy of any creature on the battlefield" (clones), "becomes a copy of target artifact until end of turn", "become a copy of that creature, except it has this ability".</summary>
/// <exampledoc>Clone</exampledoc>
/// <examplecapture>enter as a copy of any creature on the battlefield</examplecapture>
[Dependent]
public class BecomesCopy : Glyph, IPredicate
{
    public override Nib[] Nibs => [Prop(Verb), "a copy of", Prop(Original), Prop(Duration)];

    public CopyingVerb Verb { get; set; }
    public OneOf<CopiedWithExceptions, OneOf<PermanentPhrase, IndefinitePermanent>, AnyObjectInZone> Original { get; set; }
    [Optional]
    public EffectDuration Duration { get; set; }
}
