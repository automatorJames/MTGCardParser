namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A spell cast by the effect just described: "a spell cast this way", "each spell cast this way", "a spell cast by an opponent this way" - what a following cost change applies to.</summary>
/// <exampledoc>Elite Spellbinder</exampledoc>
/// <examplecapture>a spell cast this way</examplecapture>
[Dependent]
public class SpellCastThisWay : Glyph
{
    public override Nib[] Nibs => [Alt("a", "each"), "spell cast", Prop(Caster), "this way"];

    [Optional]
    public SpellCastBy Caster { get; set; }
}
