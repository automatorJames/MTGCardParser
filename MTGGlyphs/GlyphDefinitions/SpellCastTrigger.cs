namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A trigger on a player casting a spell, optionally of a color or type: "whenever a player casts a red spell, …", "whenever you cast an enchantment spell, …".</summary>
/// <exampledoc>Iron Star</exampledoc>
/// <examplecapture>whenever a player casts a red spell, you may pay {1}</examplecapture>
public class SpellCastTrigger : Glyph
{
    public override Nib[] Nibs => ["whenever", Prop(Caster), Pattern("casts?"), Prop(Spell), ",", Prop(Effect)];

    public WhichPlayer Caster { get; set; }
    public IndefiniteSpell Spell { get; set; }
    [AllowUnmatched]
    public DynamicGlyph Effect { get; set; }
}
