namespace MTGGlyphs.GlyphDefinitions;

public class CounterOnCard : Glyph
{
    public override Nib[] Nibs => [Prop(CounterType), "counter"];

    public CounterType CounterType { get; set; }
}