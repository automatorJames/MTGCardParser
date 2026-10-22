namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A Class enchantment's level-up ability: "{1}{b}: level 2" - pay the cost to gain the next level (activated only as a sorcery, by the rules).</summary>
/// <exampledoc>Bandit's Talent</exampledoc>
/// <examplecapture>{b}: level 2</examplecapture>
public class ClassLevelUp : Glyph
{
    public override Nib[] Nibs => [Prop(Cost), ": level", Prop(Level)];

    public ManaCost Cost { get; set; }
    public int Level { get; set; }
}
