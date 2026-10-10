namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A count, as an amount: "the number of swamps you control", "the number of doom counters on it", "the number of cards in your hand".</summary>
/// <exampledoc>Nightmare</exampledoc>
/// <examplecapture>the number of swamps you control</examplecapture>
[Dependent]
public class NumberOf : Glyph
{
    public override Nib[] Nibs => ["the number of", Prop(Counted)];

    public OneOf<Permanents, CountersOn, CardsInZone> Counted { get; set; }
}
