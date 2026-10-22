namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Compleated's second reminder sentence: "if life was paid, this planeswalker enters with two fewer loyalty counters".</summary>
/// <exampledoc>Jace, the Perfected Mind</exampledoc>
/// <examplecapture>if life was paid, this planeswalker enters with two fewer loyalty counters</examplecapture>
public class CompleatedRule : Glyph
{
    public override Nib[] Nibs => ["if life was paid,", Prop(Planeswalker), "enters with two fewer loyalty counters"];

    public ThisCreature Planeswalker { get; set; }
}
