namespace MTGGlyphs.GlyphDefinitions;

/// <summary>An ability a player activates, as a trigger names it: "an ability", "a loyalty ability", "a ninjutsu ability", "an eternalize or embalm ability", "an ability of an elemental", "a loyalty ability of a chandra planeswalker", "an ability of a creature or land that isn't a mana ability".</summary>
/// <exampledoc>Ceaseless Searblades</exampledoc>
/// <examplecapture>an ability of an elemental</examplecapture>
[Dependent]
public class ActivatedAbilityDescriptor : Glyph
{
    public override Nib[] Nibs => [Pattern("an?"), Prop(Kind), "ability", Prop(Source), Prop(NonMana)];

    [Optional]
    public OneOf<AbilityKindWord?, ManyOf<Keyword?>> Kind { get; set; }
    [Optional]
    public AbilitySource Source { get; set; }
    [RegexPattern("that isn't a mana ability")]
    public bool NonMana { get; set; }
}
