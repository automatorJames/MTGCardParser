namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Gaining or losing a quoted ability: 'loses "enchant creature card in a graveyard"'.</summary>
/// <exampledoc>Animate Dead</exampledoc>
/// <examplecapture>loses "enchant creature card in a graveyard"</examplecapture>
public class GainOrLoseAbility : Glyph
{
    public override Nib[] Nibs => [Prop(GainOrLose), "\"", Prop(Ability), "\""];

    public GainOrLose GainOrLose { get; set; }
    
    [RegexPattern("[^\"]+")]
    public DynamicGlyph Ability { get; set; }
}