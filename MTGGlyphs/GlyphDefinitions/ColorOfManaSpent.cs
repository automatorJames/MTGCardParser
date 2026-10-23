namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"color of mana spent to cast it", "color of mana spent to cast this spell", "color of mana spent to cast that spell": the colors of mana paid for a spell, as sunburst and converge count them.</summary>
/// <exampledoc>Etched Oracle</exampledoc>
/// <examplecapture>color of mana spent to cast it</examplecapture>
[Dependent]
public class ColorOfManaSpent : Glyph
{
    public override Nib[] Nibs => ["color of mana spent", Prop(Spell)];

    public SpellPaidFor Spell { get; set; }
}
