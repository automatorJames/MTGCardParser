namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A cost that must be paid as well as casting a spell: "as an additional cost to cast this spell, sacrifice a creature", or as well as casting spells of a kind: "as an additional cost to cast red permanent spells, you may pay 2 life" (the Defilers).</summary>
/// <exampledoc>Metamorphosis</exampledoc>
/// <examplecapture>as an additional cost to cast this spell, sacrifice a creature</examplecapture>
public class AdditionalCastingCost : Glyph
{
    public override Nib[] Nibs => ["as an additional cost to cast", Prop(Spells), ",", Prop(Cost)];

    [Referent]
    public OneOf<ThisSpell, SpellsCast> Spells { get; set; }
    public DynamicGlyph Cost { get; set; }
}
