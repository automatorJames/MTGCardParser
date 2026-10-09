namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Countering a spell: "counter target spell", "counter target red spell".</summary>
/// <exampledoc>Counterspell</exampledoc>
/// <examplecapture>counter target spell</examplecapture>
public class CounterSpell : Glyph
{
    public override Nib[] Nibs => ["counter", Prop(Countered)];

    public TargetSpell Countered { get; set; }
}
