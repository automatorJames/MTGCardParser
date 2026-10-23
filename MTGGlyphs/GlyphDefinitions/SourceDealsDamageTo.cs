namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Any source dealing damage as a trigger condition: "a source deals damage to this creature", "a source deals damage to you", "a source an opponent controls deals damage to you or a permanent you control", "a source you control deals damage to another player", a spell as the source ("an instant or sorcery spell you control deals damage", "a spell deals damage to this creature"). The source is a referent, so the effect can say "that source's controller".</summary>
/// <exampledoc>Phyrexian Obliterator</exampledoc>
/// <examplecapture>a source deals damage to this creature</examplecapture>
[Dependent]
public class SourceDealsDamageTo : Glyph
{
    public override Nib[] Nibs => [Pattern("an?"), Prop(Source), Prop(Controller), "deals damage", Prop(Recipient)];

    [Referent]
    public OneOf<DamageSourceNoun, PlayableCards> Source { get; set; }
    [Optional]
    public ControlledBy Controller { get; set; }
    [Optional]
    public DamageRecipient Recipient { get; set; }
}
