namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A trigger on counters being put on or removed from a permanent: "whenever a +1/+1 counter is put on this creature, …", "whenever one or more +1/+1 counters are put on another creature you control, …", "whenever a counter is put on a creature you control, …", "whenever a +1/+1 counter is removed from this creature, …", "whenever one or more loyalty counters are put on planeswalkers you control, …", "… for the first time each turn, …", "when the fourth plan counter is put on this enchantment, …".</summary>
/// <exampledoc>Fathom Mage</exampledoc>
/// <examplecapture>whenever a +1/+1 counter is put on this creature, you may draw a card</examplecapture>
public class CountersPutTrigger : Glyph
{
    public override Nib[] Nibs => [Alt("when", "whenever"), Prop(Counters), Prop(Movement), Prop(Permanent), Prop(Period), ",", Prop(Effect)];

    public OneOf<Counters, NthCounter> Counters { get; set; }
    public CounterMovement Movement { get; set; }
    public OneOf<PermanentPhrase, IndefinitePermanent, OneOf<OneOrMorePermanents, Permanents, ShortName>> Permanent { get; set; }
    [Optional]
    public OneOf<ThisPeriod, PlayerEventTiming?> Period { get; set; }
    public DynamicGlyph Effect { get; set; }
}
