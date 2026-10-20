namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Something done as an event happens, not after it: "as {this} enters, choose an opponent", "as this land enters, you may pay 2 life", "as this creature attacks, you may tap …" (enlist), "as you cast this spell, you may sacrifice a creature with power 1 or greater" (casualty).</summary>
/// <exampledoc>Black Vise</exampledoc>
/// <examplecapture>as {this} enters, choose an opponent</examplecapture>
public class AsThisEnters : Glyph
{
    public override Nib[] Nibs => ["as", Prop(Moment), ",", Prop(Effect)];

    public OneOf<PermanentAsEvent, CastingThisSpell> Moment { get; set; }
    public DynamicGlyph Effect { get; set; }
}
