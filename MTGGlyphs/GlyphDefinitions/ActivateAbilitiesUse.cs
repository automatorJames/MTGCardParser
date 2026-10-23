namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Spending mana to activate abilities: "activate abilities", "activate abilities of artifacts", "activate an ability of an elemental", abilities of a keyword ("activate equip abilities", "activate an equip ability"), or "to activate …" after "or".</summary>
/// <exampledoc>Renowned Weaponsmith</exampledoc>
/// <examplecapture>activate abilities of artifacts</examplecapture>
[Dependent]
public class ActivateAbilitiesUse : Glyph
{
    public override Nib[] Nibs => [Opt("to"), "activate", Prop(Which), Prop(Owner)];

    public OneOf<KeywordAbilities, AbilitiesWord?> Which { get; set; }
    [Optional]
    public AbilitiesOf Owner { get; set; }
}
