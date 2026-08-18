namespace MTGGlyphs;

[RegexBoundaryOptionAtrribute(BoundaryOption.None)]
public class WhereVariableIs : Glyph
{
    public override Nib[] Nibs => [", where", Prop(VariableName), "is "];

    public VariableName VariableName { get; set; }
}