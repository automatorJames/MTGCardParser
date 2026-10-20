namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A restriction on how mana just produced may be spent: "spend this mana only to cast a creature spell", "this mana can't be spent to cast a nonartifact spell". The rule says whether the spells named are the only use or the forbidden one.</summary>
/// <exampledoc>Ancient Ziggurat</exampledoc>
/// <examplecapture>spend this mana only to cast a creature spell</examplecapture>
public class ManaSpendingRestriction : Glyph
{
    public override Nib[] Nibs => [Prop(Rule), "to cast", Prop(Spells)];

    public ManaSpendingRule Rule { get; set; }
    public OneOf<IndefiniteSpell, PlayableCards> Spells { get; set; }
}
