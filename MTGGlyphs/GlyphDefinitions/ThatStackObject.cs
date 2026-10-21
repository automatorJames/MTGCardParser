namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A spell or ability on the stack named earlier: "that spell", "that spell or ability", "that ability", "the spell".</summary>
/// <exampledoc>Glyph Keeper</exampledoc>
/// <examplecapture>that spell or ability</examplecapture>
[Dependent]
public class ThatStackObject : Glyph
{
    public override Nib[] Nibs => [Alt("that", "the"), Prop(Kind)];

    public StackObjectKind Kind { get; set; }
}
