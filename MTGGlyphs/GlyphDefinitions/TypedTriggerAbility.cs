namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A triggered ability whose effect contains commas ("when you cast this spell, exile target artifact, enchantment, or land", "when you cycle this card, search your library for …, reveal it, put it into your hand, then shuffle"). TriggeredAbility's open trigger slot runs to the last comma and fails there; typing the trigger (casting this spell, a player event, or a subject doing something) ends it at the first comma. Tried after other glyphs, so TriggeredAbility keeps its lines.</summary>
/// <exampledoc>World Breaker</exampledoc>
/// <examplecapture>when you cast this spell, exile target artifact, enchantment, or land</examplecapture>
[TokenizationOrder(-1)]
public class TypedTriggerAbility : Glyph
{
    public override Nib[] Nibs => [Alt("when", "whenever"), Prop(Trigger), ",", Prop(Effect)];

    public OneOf<CastingThisSpell, PlayerEvent, SubjectDoes> Trigger { get; set; }
    public DynamicGlyph Effect { get; set; }
}
