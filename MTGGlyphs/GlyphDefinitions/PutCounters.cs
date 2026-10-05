namespace MTGGlyphs.GlyphDefinitions;

public class PutCounters : Glyph
{
    public override Nib[] Nibs => ["put", Prop(Counters), "on", Prop(Recipient)];

    public Counters Counters { get; set; }
    public Recipient Recipient { get; set; }
}
