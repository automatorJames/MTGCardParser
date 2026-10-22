namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player controlling an exact number of permanents of a kind, as a condition: "you control exactly one creature".</summary>
/// <exampledoc>Demonic Rising</exampledoc>
/// <examplecapture>you control exactly one creature</examplecapture>
[Dependent]
public class ControlsExactly : Glyph
{
    public override Nib[] Nibs => [Prop(Controller), Pattern("controls?"), "exactly", Prop(Count), Plural(Prop(Kind))];

    public WhichPlayer Controller { get; set; }
    public Quantity Count { get; set; }
    public CardType Kind { get; set; }
}
