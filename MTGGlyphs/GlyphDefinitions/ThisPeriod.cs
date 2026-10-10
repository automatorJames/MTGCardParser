namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"this turn", "this combat": an effect lasting for the rest of the current turn or combat.</summary>
/// <exampledoc>Hurr Jackal</exampledoc>
/// <examplecapture>this turn</examplecapture>
[Dependent]
public class ThisPeriod : Glyph
{
    public override Nib[] Nibs => ["this", Prop(Period)];

    public TurnPeriod Period { get; set; }
}
