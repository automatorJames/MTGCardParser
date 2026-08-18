namespace MTGGlyphs;

public class GainOrLoseAbility : Glyph
{
    public override Nib[] Nibs => [Prop(GainOrLose), "\"", Prop(Ability), "\""];

    public GainOrLose GainOrLose { get; set; }
    
    [RegexPattern("[^\"]+")]
    public DynamicGlyph Ability { get; set; }
}