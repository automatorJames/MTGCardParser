namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"to activate those abilities", "to activate abilities of creatures you control": activated abilities that spent mana pays for.</summary>
/// <exampledoc>Agatha's Soul Cauldron</exampledoc>
/// <examplecapture>to activate abilities of creatures you control</examplecapture>
[Dependent]
public class SpendToActivate : Glyph
{
    public override Nib[] Nibs => ["to activate", Prop(Abilities)];

    public OneOf<ActivatedAbilitiesOfGroup, AbilitiesOfGroup, ThoseAbilities?> Abilities { get; set; }
}
