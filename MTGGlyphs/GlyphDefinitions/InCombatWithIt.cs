namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"it's blocking", "it's attacking": a qualifier naming the creatures a creature named earlier is in combat with ("each creature it's blocking"). Its pronoun is captured, as CombatRelation's object is.</summary>
/// <exampledoc>Guardian of the Gateless</exampledoc>
/// <examplecapture>it's blocking</examplecapture>
[Dependent]
public class InCombatWithIt : Glyph
{
    public override Nib[] Nibs => [Prop(Other), "'s", Prop(Status)];

    public It Other { get; set; }
    public InCombatStatus Status { get; set; }
}
