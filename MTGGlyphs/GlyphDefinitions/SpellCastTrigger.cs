namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A trigger on a player casting a spell, optionally of a color or type, and optionally narrowed after: "whenever a player casts a red spell, …", "whenever you cast an enchantment spell, …", "whenever you cast a spell that targets this creature, …", "whenever you cast a spell from your graveyard, …", "whenever you cast a spell during an opponent's turn, …", "whenever you cast or copy an instant or sorcery spell, …" (magecraft), "whenever you cast your second spell each turn, …".</summary>
/// <exampledoc>Iron Star</exampledoc>
/// <examplecapture>whenever a player casts a red spell, you may pay {1}</examplecapture>
public class SpellCastTrigger : Glyph
{
    public override Nib[] Nibs => ["whenever", Prop(Caster), Prop(Verb), Prop(Spell), Prop(Narrowed), ",", Prop(Effect)];

    public WhichPlayer Caster { get; set; }
    public CastVerb Verb { get; set; }
    public OneOf<IndefiniteSpell, NthSpellThisTurn> Spell { get; set; }
    [Optional]
    public SpellNarrowing Narrowed { get; set; }
    public DynamicGlyph Effect { get; set; }
}
