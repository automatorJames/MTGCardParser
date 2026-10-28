namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A triggered ability whose trigger condition isn't a modelled event frame: "whenever a player taps a land for mana, {this} deals 1 damage to that player", "when you cast this spell, exile target artifact, enchantment, or land", "whenever you gain life, …". The trigger is typed where it can be - casting this spell or a player event - which bounds it at the first comma, so the effect can be a multi-comma sequence; anything else takes the text up to the last comma before the effect. Tried after the specific trigger frames.</summary>
/// <exampledoc>Manabarbs</exampledoc>
/// <examplecapture>whenever a player taps a land for mana, {this} deals 1 damage to that player</examplecapture>
[TokenizationOrder(-1)]
public class TriggeredAbility : Glyph
{
    public override Nib[] Nibs => [Alt("when", "whenever"), Prop(Condition), ",", Prop(Effect)];

    public OneOf<CastingThisSpell, PlayerEvent, DynamicGlyph> Condition { get; set; }
    public DynamicGlyph Effect { get; set; }
}
