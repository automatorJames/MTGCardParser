namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A replacement effect on putting counters: "if you would put one or more counters on a permanent or player, put twice that many of each of those kinds of counters on that permanent or player instead". Captures who places them, what and where, and how the number changes (doubled, tripled, plus one, halved with rounding).</summary>
/// <exampledoc>Vorinclex, Monstrous Raider</exampledoc>
/// <examplecapture>if you would put one or more counters on a permanent or player, put twice that many of each of those kinds of counters on that permanent or player instead</examplecapture>
public class CounterPlacementReplacement : Glyph
{
    public override Nib[] Nibs => ["if", Prop(Placer), "would put", Prop(Counters), "on", Prop(Recipient), ",", Prop(Replacer), Pattern("puts?"), Prop(Increase), Prop(Kinds), "on", Prop(NewRecipient), Prop(Replacement)];

    public OneOf<WhichPlayer?, CounterPlacer?> Placer { get; set; }
    public Counters Counters { get; set; }
    public OneOf<PermanentOrPlayer, CounterRecipient> Recipient { get; set; }
    [Optional]
    public CounterPlacer? Replacer { get; set; }
    public CounterIncrease Increase { get; set; }
    public OneOf<CountersOfKind, EachOfThoseKindsOfCounters> Kinds { get; set; }
    public OneOf<ThatPermanentOrPlayer, CounterRecipient> NewRecipient { get; set; }
    public OneOf<InsteadRounded, Instead?> Replacement { get; set; }
}
