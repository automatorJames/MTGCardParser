namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Three effects done in order: "look at the top four cards of your library, exile one face down, then put the rest on the bottom in a random order" (hideaway). Like <see cref="ThenSequence"/>, tried after other glyphs; each step resolves on its own.</summary>
/// <exampledoc>Howltooth Hollow</exampledoc>
/// <examplecapture>look at the top four cards of your library, exile one face down, then put the rest on the bottom in a random order</examplecapture>
[TokenizationOrder(-1)]
public class ThreeStepSequence : Glyph
{
    public override Nib[] Nibs => [Prop(First), ",", Prop(Second), ", then", Prop(Next)];

    public DynamicGlyph First { get; set; }
    public DynamicGlyph Second { get; set; }
    public DynamicGlyph Next { get; set; }
}
