namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"by artifact creatures", "by artifact sources", "by target creature", "by that creature", "by enchanted creature", "by a source of your choice", "by target instant or sorcery spell", "by instant and sorcery spells": the sources a prevention or redirection effect applies to - the attached permanent, a group, one particular permanent, a source the player picks, or spells.</summary>
/// <exampledoc>Argothian Pixies</exampledoc>
/// <examplecapture>by artifact creatures</examplecapture>
[Dependent]
public class DamageSource : Glyph
{
    public override Nib[] Nibs => ["by", Prop(Sources)];

    public OneOf<EnchantedPermanent, Permanents, DamageSources, PermanentPhrase, ChosenSource, TargetSpell, PlayableCards> Sources { get; set; }
}
