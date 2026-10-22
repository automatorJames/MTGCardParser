namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A quoted ability, the closing quote falling after the sentence's period: `"pay 3 life: regenerate this creature`.</summary>
/// <exampledoc>Anavolver</exampledoc>
/// <examplecapture>"pay 3 life: regenerate this creature</examplecapture>
[Dependent]
public class QuotedAbility : Glyph
{
    public override Joiner Joiner => Joiner.None;
    public override Nib[] Nibs => ["\"", Prop(Ability)];

    public DynamicGlyph Ability { get; set; }
}
