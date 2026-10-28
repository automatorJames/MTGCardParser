namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A static ability that stops permanents' movements from triggering abilities: "creatures entering don't cause abilities to trigger", "artifacts and creatures entering the battlefield don't cause abilities to trigger", "creatures entering or dying don't cause abilities to trigger" (Hushwing Gryff, Hushbringer, Doorkeeper Thrull).</summary>
/// <exampledoc>Hushwing Gryff</exampledoc>
/// <examplecapture>creatures entering don't cause abilities to trigger</examplecapture>
public class MovementsDontTrigger : Glyph
{
    public override Nib[] Nibs => [Prop(Kinds), Prop(Movements), "don't cause abilities to trigger"];

    public OneOf<ManyOf<PluralTargetKind>, PluralTargetKind> Kinds { get; set; }
    public OneOf<ManyOf<TriggeringEvent>, TriggeringEvent?> Movements { get; set; }
}
