namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Something a permanent can get, have or lose: a power/toughness change ("+1/+1"), a keyword ("flying"), protection ("protection from black"), set base stats ("base power 0"), the abilities listed after backup ("the following ability"), or all its abilities ("loses all abilities").</summary>
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
    public Keyword? Keyword { get; set; }
}
