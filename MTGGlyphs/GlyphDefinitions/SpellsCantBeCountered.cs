namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A group of spells that can't be countered, optionally only yours, optionally for this turn: "creature spells you control can't be countered", "spells you control can't be countered", "instant and sorcery spells you control can't be countered", "creature spells can't be countered", "sliver spells can't be countered", "spells you control can't be countered this turn".</summary>
/// <exampledoc>Prowling Serpopard</exampledoc>
/// <examplecapture>creature spells you control can't be countered</examplecapture>
public class SpellsCantBeCountered : Glyph
{
    public override Nib[] Nibs => [Prop(Spells), Prop(YouControl), "can't be countered", Prop(ThisTurn)];

    public SpellsCast Spells { get; set; }
    [RegexPattern("you control")]
    public bool YouControl { get; set; }
    [RegexPattern("this turn")]
    public bool ThisTurn { get; set; }
}
