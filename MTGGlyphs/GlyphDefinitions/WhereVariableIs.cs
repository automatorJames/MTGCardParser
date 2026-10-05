namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An effect using a variable, then the variable's definition: "you gain x life, where x is the number of cards in your hand".</summary>
public class WhereVariableIs : Glyph
{
    public override Nib[] Nibs => [Prop(Effect), ", where", Prop(VariableName), "is", Prop(Definition)];

    [AllowUnmatched]
    public DynamicGlyph Effect { get; set; }

    public VariableName VariableName { get; set; }

    [AllowUnmatched]
    public DynamicGlyph Definition { get; set; }
}
