namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"unless a player has 13 or less life", "if you have 25 or more life": a condition on the game state introduced by a connective.</summary>
/// <exampledoc>Murky Sewer</exampledoc>
/// <examplecapture>unless a player has 13 or less life</examplecapture>
[Dependent]
public class StateCondition : Glyph
{
    public override Nib[] Nibs => [Prop(Connective), Prop(State)];

    public ConditionConnective Connective { get; set; }
    public LifeThreshold State { get; set; }
}
