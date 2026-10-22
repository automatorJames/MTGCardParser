namespace MTGGlyphs.GlyphDefinitions;

/// <summary>How much mana was spent on a spell, as a condition: "at least four mana was spent to cast it", "at least three black mana was spent to cast this spell" (adamant), "at least three mana of the same color was spent to cast it", "five or more mana was spent to cast that spell" (opus), "no mana was spent to cast it", "two or more colors of mana were spent to cast it". Mana symbols ("{u} was spent to cast it") are <see cref="ManaWasSpent"/>.</summary>
/// <exampledoc>Locthwain Paladin</exampledoc>
/// <examplecapture>at least three black mana was spent to cast this spell</examplecapture>
[Dependent]
public class ManaSpentCondition : Glyph
{
    public override Nib[] Nibs => [Prop(Amount), Prop(Color), Prop(Mana), Pattern("(was|were)"), "spent to cast", Prop(Spell)];

    public OneOf<AtLeastQuantity, AtLeastCount, Quantity?> Amount { get; set; }
    [Optional]
    public ManaColor? Color { get; set; }
    public SpentManaPhrase Mana { get; set; }
    public OneOf<It, ThisSpell, ThatStackObject> Spell { get; set; }
}
