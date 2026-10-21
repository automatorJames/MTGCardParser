namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Spending mana to activate abilities: "activate abilities", "activate abilities of artifacts", "activate an ability of an elemental".</summary>
/// <exampledoc>Renowned Weaponsmith</exampledoc>
/// <examplecapture>activate abilities of artifacts</examplecapture>
[Dependent]
public class ActivateAbilitiesUse : Glyph
{
    public override Nib[] Nibs => ["activate", Alt("abilities", "an ability"), Prop(Of)];

    [Optional]
    public AbilitiesOf Of { get; set; }
}
