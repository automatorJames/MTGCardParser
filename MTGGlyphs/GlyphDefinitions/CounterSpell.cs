namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Countering a spell: "counter target spell", "counter target red spell", or one named earlier, "counter it" (ward).</summary>
/// <exampledoc>Counterspell</exampledoc>
/// <examplecapture>counter target spell</examplecapture>
public class CounterSpell : Glyph
{
    public override Nib[] Nibs => ["counter", Prop(Countered)];

    public OneOf<TargetSpell, It> Countered { get; set; }
}
