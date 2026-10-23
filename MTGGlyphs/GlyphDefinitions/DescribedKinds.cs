namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"spider hero", "bear berserker", "1/1 spirit", "4/4 hero", "phyrexian creature", "1/1 spirit creature with flying", "3/3 black ooze creature": creature types a permanent becomes or gains, several in a row, possibly with base power and toughness and a color first, and possibly the word "creature" and keywords after. The lead is one optional property, since two optional properties in a row don't match. Only creature types lead, so "artifact creature" stays with <see cref="BecomesCreature"/>.</summary>
/// <exampledoc>Spider-Suit</exampledoc>
/// <examplecapture>spider hero</examplecapture>
[Dependent]
public class DescribedKinds : Glyph
{
    public override Nib[] Nibs => [Prop(Stats), Prop(Kinds), Prop(Creature), Prop(With)];

    [Optional]
    public OneOf<ColoredStats, PowerToughness, ManaColor?> Stats { get; set; }
    [JoinedBy(Joiner.Space)]
    public CompoundOf<CreatureType> Kinds { get; set; }
    [Optional]
    [RegexPattern("creatures?")]
    public bool? Creature { get; set; }
    [Optional]
    public WithKeywords With { get; set; }
}
