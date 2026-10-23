namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An effect whose variable is defined after it and fixed as the spell is cast: "{this} deals x damage divided as you choose among any number of targets, where x is the number of mountains you control as you cast this spell". Beside WhereXIs because the trailing timing has to be a plain literal to end the definition's slot.</summary>
/// <exampledoc>Jaws of Stone</exampledoc>
/// <examplecapture>{this} deals x damage divided as you choose among any number of targets, where x is the number of mountains you control as you cast this spell</examplecapture>
[TokenizationOrder(-1)]
public class WhereXIsAsCast : Glyph
{
    public override Nib[] Nibs => [Prop(Effect), ", where x is", Prop(Definition), "as you cast this spell"];

    public DynamicGlyph Effect { get; set; }
    public DynamicGlyph Definition { get; set; }
}
