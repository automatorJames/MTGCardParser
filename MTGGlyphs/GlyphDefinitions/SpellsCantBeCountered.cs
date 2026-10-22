namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A group of your spells that can't be countered: "creature spells you control can't be countered", "spells you control can't be countered", "instant and sorcery spells you control can't be countered".</summary>
/// <exampledoc>Prowling Serpopard</exampledoc>
/// <examplecapture>creature spells you control can't be countered</examplecapture>
public class SpellsCantBeCountered : Glyph
{
    public override Nib[] Nibs => [Prop(Spells), "you control can't be countered"];

    public SpellsCast Spells { get; set; }
}
