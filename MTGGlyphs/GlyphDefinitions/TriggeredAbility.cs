namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A triggered ability whose trigger condition isn't modelled yet: "whenever a player taps a land for mana, {this} deals 1 damage to that player". Catches the effect so it resolves.</summary>
/// <exampledoc>Manabarbs</exampledoc>
/// <examplecapture>whenever a player taps a land for mana, {this} deals 1 damage to that player</examplecapture>
public class TriggeredAbility : Glyph
{
    public override Nib[] Nibs => [Alt("when", "whenever"), Prop(Condition), ",", Prop(Effect)];

    [AllowUnmatched]
    public DynamicGlyph Condition { get; set; }
    [AllowUnmatched]
    public DynamicGlyph Effect { get; set; }
}
