namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A player casting a spell, as one trigger event of a <see cref="CompoundTrigger"/>: "whenever you cast a spell with mana value 4 or greater", "whenever an opponent casts a spell during your turn".</summary>
/// <exampledoc>Up the Beanstalk</exampledoc>
/// <examplecapture>whenever you cast a spell with mana value 5 or greater</examplecapture>
[Dependent]
public class SpellTriggerEvent : Glyph
{
    public override Nib[] Nibs => [Alt("when", "whenever"), Prop(Caster), Prop(Verb), Prop(Spell), Prop(Narrowed)];

    public WhichPlayer Caster { get; set; }
    public CastVerb Verb { get; set; }
    public OneOf<IndefiniteSpell, NthSpellThisTurn> Spell { get; set; }
    [Optional]
    public SpellNarrowing Narrowed { get; set; }
}
