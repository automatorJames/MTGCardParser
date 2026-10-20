namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"your life total": the life a player has, as an amount.</summary>
/// <exampledoc>Ajani, Caller of the Pride</exampledoc>
/// <examplecapture>your life total</examplecapture>
[Dependent]
public class LifeTotalOf : Glyph
{
    public override Nib[] Nibs => [Prop(Whose), "life total"];

    public Whose Whose { get; set; }
}
