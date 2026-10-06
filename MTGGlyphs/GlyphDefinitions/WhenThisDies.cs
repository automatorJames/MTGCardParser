namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A dies trigger. The literal {this} is a referent, so an "it" in the effect resolves to the card.</summary>
/// <exampledoc>Onulet</exampledoc>
/// <examplecapture>when {this} dies, you gain 2 life</examplecapture>
public class WhenThisDies : Glyph
{
    public override Nib[] Nibs => ["when {this} dies,", Prop(Effect)];

    public DynamicGlyph Effect { get; set; }
}
