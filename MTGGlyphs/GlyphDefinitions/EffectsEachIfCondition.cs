namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Two effects each done only if its own condition holds, joined by "and": "creatures your opponents control lose flying until end of turn if {g} was spent to cast this spell, and creatures you control gain flying until end of turn if {u} was spent to cast this spell", "target player can't play lands this turn if {r} was spent to cast this spell and can't cast creature spells this turn if {w} was spent to cast this spell" (the hybrid "do both" spells).</summary>
/// <exampledoc>Invert the Skies</exampledoc>
/// <examplecapture>creatures your opponents control lose flying until end of turn if {g} was spent to cast this spell, and creatures you control gain flying until end of turn if {u} was spent to cast this spell</examplecapture>
public class EffectsEachIfCondition : Glyph
{
    public override Nib[] Nibs => [Prop(First), Pattern(",? and"), Prop(Second)];

    public EffectIfCondition First { get; set; }
    public EffectIfCondition Second { get; set; }
}
