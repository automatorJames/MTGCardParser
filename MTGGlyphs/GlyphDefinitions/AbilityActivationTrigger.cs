namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A trigger on a player activating an ability: "whenever you activate a loyalty ability, …", "whenever you activate an ability of an elemental, …", "whenever an opponent activates an ability that isn't a mana ability, …".</summary>
/// <exampledoc>Ceaseless Searblades</exampledoc>
/// <examplecapture>whenever you activate an ability of an elemental, this creature gets +1/+0 until end of turn</examplecapture>
public class AbilityActivationTrigger : Glyph
{
    public override Nib[] Nibs => ["whenever", Prop(Activator), Pattern("activates?"), Prop(Ability), ",", Prop(Effect)];

    public WhichPlayer Activator { get; set; }
    public ActivatedAbilityDescriptor Ability { get; set; }
    public DynamicGlyph Effect { get; set; }
}
