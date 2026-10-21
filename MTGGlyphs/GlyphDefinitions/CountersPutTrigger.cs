namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A trigger on counters being put on a permanent: "whenever a +1/+1 counter is put on this creature, …", "whenever one or more +1/+1 counters are put on another creature you control, …", "whenever a counter is put on a creature you control, …".</summary>
/// <exampledoc>Fathom Mage</exampledoc>
/// <examplecapture>whenever a +1/+1 counter is put on this creature, you may draw a card</examplecapture>
public class CountersPutTrigger : Glyph
{
    public override Nib[] Nibs => [Alt("when", "whenever"), Prop(Counters), Alt("is put on", "are put on"), Prop(Permanent), Prop(Period), ",", Prop(Effect)];

    public Counters Counters { get; set; }
    public OneOf<PermanentPhrase, IndefinitePermanent> Permanent { get; set; }
    [Optional]
    public ThisPeriod Period { get; set; }
    public DynamicGlyph Effect { get; set; }
}
