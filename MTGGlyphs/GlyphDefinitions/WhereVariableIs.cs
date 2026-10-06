namespace MTGGlyphs.GlyphDefinitions;

/// <summary>", where x is": introduces what a variable in the text stands for.</summary>
/// <exampledoc>Berserk</exampledoc>
/// <examplecapture>, where x is</examplecapture>
public class WhereVariableIs : Glyph
{
    public override Nib[] Nibs => [", where", Prop(VariableName), "is "];

    public VariableName VariableName { get; set; }
}