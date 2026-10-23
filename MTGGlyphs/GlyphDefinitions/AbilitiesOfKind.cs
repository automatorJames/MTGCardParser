namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Abilities of one keyword or class, as what a cost change applies to: "equip abilities you activate", "cycling abilities you activate", "loyalty abilities of planeswalkers your opponents control", "exhaust abilities of other permanents you control", "equip abilities you activate that target this creature", "activated abilities", "abilities your opponents activate", "abilities you activate that aren't mana abilities".</summary>
/// <exampledoc>Fighter Class</exampledoc>
/// <examplecapture>equip abilities you activate</examplecapture>
[Dependent]
public class AbilitiesOfKind : Glyph
{
    public override Nib[] Nibs => [Prop(Kind), "abilities", Prop(Scope), Prop(NonMana), Prop(Targeting)];

    [Optional]
    public OneOf<Keyword?, AbilityClass?> Kind { get; set; }
    [Optional]
    public OneOf<YouActivate?, AbilitiesOwner> Scope { get; set; }
    [Optional]
    [RegexPattern("that aren't mana abilities")]
    public bool? NonMana { get; set; }
    [Optional]
    public ThatTarget Targeting { get; set; }
}
