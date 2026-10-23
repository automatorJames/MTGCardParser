namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Something a permanent can get, have or lose: a power/toughness change ("+1/+1"), a keyword ("flying"), a keyword with a number ("bushido 1", "toxic 1") or a cost ("ward {2}"), affinity ("affinity for artifacts"), landwalk of a chosen type ("landwalk of the chosen type"), protection ("protection from black"), set base stats ("base power 0"), the abilities listed after backup ("the following ability"), all its abilities ("loses all abilities"), the ability itself ("loses this ability"), all creature types, a choice of keywords ("your choice of flying or lifelink"), or a chosen ability ("that ability").</summary>
/// <exampledoc>Blessing</exampledoc>
/// <examplecapture>+1/+1</examplecapture>
[Dependent]
public class Buff : GlyphOneOf
{
    public PowerToughnessMod PowerToughnessMod { get; set; }
    public Protection Protection { get; set; }
    public BasePowerToughness BasePowerToughness { get; set; }
    public FollowingAbilities FollowingAbilities { get; set; }
    public AllAbilities AllAbilities { get; set; }
    public ThisAbility ThisAbility { get; set; }
    public AllCreatureTypes AllCreatureTypes { get; set; }
    public ChoiceOfBuffs ChoiceOfBuffs { get; set; }
    public ChosenAbility ChosenAbility { get; set; }
    public KeywordWithAmount KeywordWithAmount { get; set; }
    public KeywordWithCost KeywordWithCost { get; set; }
    public AffinityFor Affinity { get; set; }
    public ChosenTypeLandwalk ChosenTypeLandwalk { get; set; }
    public Keyword? Keyword { get; set; }
}
