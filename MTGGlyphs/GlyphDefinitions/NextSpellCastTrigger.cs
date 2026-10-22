namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A delayed trigger on the next spell cast this turn: "when you next cast an instant or sorcery spell this turn, copy that spell", "when you cast your next instant or sorcery spell this turn, …", "when you next cast an instant or sorcery spell from your hand this turn, it gains rebound".</summary>
/// <exampledoc>Howl of the Horde</exampledoc>
/// <examplecapture>when you next cast an instant or sorcery spell this turn, copy that spell</examplecapture>
public class NextSpellCastTrigger : Glyph, IPredicate
{
    public override Nib[] Nibs => [Alt("when you next cast", "when you cast your next"), Prop(Spell), Prop(Narrowed), "this turn,", Prop(Effect)];

    public OneOf<IndefiniteSpell, QualifiedSpellNoun> Spell { get; set; }
    [Optional]
    public SpellNarrowing Narrowed { get; set; }
    public DynamicGlyph Effect { get; set; }
}
