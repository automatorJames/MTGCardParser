namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A trigger on one of a player's spells being countered: "a spell you've cast is countered".</summary>
/// <exampledoc>Multani's Presence</exampledoc>
/// <examplecapture>a spell you've cast is countered</examplecapture>
[Dependent]
public class SpellCounteredEvent : Glyph
{
    public override Nib[] Nibs => ["a spell", Prop(Caster), "cast is countered"];

    public PerfectSubject Caster { get; set; }
}
