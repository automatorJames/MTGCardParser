namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Any source dealing damage as a trigger condition: "a source deals damage to this creature", "a source deals damage to you", "a source an opponent controls deals damage to you or a permanent you control". The source is a referent, so the effect can say "that source's controller". Resolved inside TriggeredAbility's condition.</summary>
/// <exampledoc>Phyrexian Obliterator</exampledoc>
/// <examplecapture>a source deals damage to this creature</examplecapture>
[Dependent]
public class SourceDealsDamageTo : Glyph
{
    public override Nib[] Nibs => ["a", Prop(Source), Prop(Controller), "deals damage to", Prop(Recipient)];

    [Referent]
    public DamageSourceNoun Source { get; set; }
    [Optional]
    public ControlledBy Controller { get; set; }
    public OneOf<PermanentPhrase, WhichPlayer?, YouOrAPermanentYouControl> Recipient { get; set; }
}
