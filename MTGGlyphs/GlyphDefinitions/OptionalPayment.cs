namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An optional payment as a sentence of its own, usually followed by "if you do, …" or "if they do, …": "you may pay {g}", "that player may pay 10 life", "you may pay {2} for each wage counter on it".</summary>
/// <exampledoc>Forgotten Lore</exampledoc>
/// <examplecapture>you may pay {g}</examplecapture>
public class OptionalPayment : Glyph
{
    public override Nib[] Nibs => [Prop(Payment)];

    public OptionalPayCost Payment { get; set; }
}
