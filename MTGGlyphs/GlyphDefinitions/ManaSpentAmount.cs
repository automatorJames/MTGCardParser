namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the amount of mana spent to cast that spell", "the amount of {s} spent to cast this spell": how much mana (or snow mana) paid for a spell, as an amount.</summary>
/// <exampledoc>Memory Deluge</exampledoc>
/// <examplecapture>the amount of mana spent to cast this spell</examplecapture>
[Dependent]
public class ManaSpentAmount : Glyph
{
    public override Nib[] Nibs => ["the amount of", Prop(Kind), "spent to cast", Prop(Spell)];

    public SpentManaKind Kind { get; set; }
    public OneOf<It, ThisSpell, ThatStackObject, ThisCreature> Spell { get; set; }
}
