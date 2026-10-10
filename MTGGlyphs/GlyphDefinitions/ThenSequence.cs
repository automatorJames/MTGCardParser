namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Two effects done in order: "draw a card, then discard a card".</summary>
/// <exampledoc>Jalum Tome</exampledoc>
/// <examplecapture>draw a card, then discard a card</examplecapture>
[Dependent]
public class ThenSequence : Glyph
{
    public override Nib[] Nibs => [Prop(First), ", then", Prop(Next)];

    public DynamicGlyph First { get; set; }
    public DynamicGlyph Next { get; set; }
}
