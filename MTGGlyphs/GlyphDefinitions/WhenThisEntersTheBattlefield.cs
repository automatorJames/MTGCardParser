namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An enters-the-battlefield trigger in which the card gains or loses quoted abilities.</summary>
/// <exampledoc>Animate Dead</exampledoc>
/// <examplecapture>when {this} enters the battlefield, if it's on the battlefield, it loses "enchant creature card in a graveyard" and gains "enchant creature put onto the battlefield with {this}"</examplecapture>
public class WhenThisEntersTheBattlefield : Glyph
{
    public override Nib[] Nibs => ["when", Nib.This, "enters the battlefield,", Prop(MustStillBeOnTheBattlefield), "it", Prop(GainedOrLostAbilities)];

    [RegexPattern("if it's on the battlefield,")]
    public bool MustStillBeOnTheBattlefield { get; set; }

    public ManyOf<GainOrLoseAbility> GainedOrLostAbilities { get; set; }
}