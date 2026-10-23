namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Spells cast by the effect just described: "spells this way", "a spell this way" - what an any-mana rider applies to.</summary>
/// <exampledoc>Black Cat, Cunning Thief</exampledoc>
/// <examplecapture>spells this way</examplecapture>
[Dependent]
public class SpellsCastThisWay : Glyph
{
    public override Nib[] Nibs => [Alt("a spell", "spells"), Prop(Way)];

    public ThisWay Way { get; set; }
}
