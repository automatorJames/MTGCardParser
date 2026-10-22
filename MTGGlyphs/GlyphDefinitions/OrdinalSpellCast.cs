namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"the second spell you cast each turn", "the first creature spell you cast each turn", "the next spell you cast this turn": one spell picked out by its order among the spells a player casts in a turn.</summary>
/// <exampledoc>Highspire Bell-Ringer</exampledoc>
/// <examplecapture>the second spell you cast each turn</examplecapture>
[Dependent]
public class OrdinalSpellCast : Glyph
{
    public override Nib[] Nibs => ["the", Prop(Ordinal), Prop(Spell), "you cast", Prop(Scope)];

    public SpellOrdinal Ordinal { get; set; }
    public OneOf<QualifiedSpellNoun, SpellNoun> Spell { get; set; }
    public SpellTurnScope Scope { get; set; }
}
