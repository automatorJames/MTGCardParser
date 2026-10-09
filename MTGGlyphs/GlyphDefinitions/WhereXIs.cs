namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An effect with a variable defined after it: "you gain x life, where x is the number of cards in your hand minus 4". The definition is held as text until something models it. Tried after other glyphs, so a trigger keeps its frame and holds this as its effect.</summary>
/// <exampledoc>Ivory Tower</exampledoc>
/// <examplecapture>you gain x life, where x is the number of cards in your hand minus 4</examplecapture>
[TokenizationOrder(-1)]
public class WhereXIs : Glyph
{
    public override Nib[] Nibs => [Prop(Effect), ", where x is", Prop(Definition)];

    public DynamicGlyph Effect { get; set; }
    [AllowUnmatched]
    public DynamicGlyph Definition { get; set; }
}
