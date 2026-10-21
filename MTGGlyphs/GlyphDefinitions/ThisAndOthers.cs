namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A trigger subject naming the card itself together with a number of others (battalion): "this creature and at least two other creatures", "tajic and at least two other creatures".</summary>
/// <exampledoc>Boros Elite</exampledoc>
/// <examplecapture>this creature and at least two other creatures</examplecapture>
[Dependent]
public class ThisAndOthers : Glyph
{
    public override Nib[] Nibs => [Prop(Self), "and at least", Prop(Count), "other", Prop(Others)];

    public OneOf<ThisCreature, This, ShortName> Self { get; set; }
    public Quantity Count { get; set; }
    public Permanents Others { get; set; }
}
