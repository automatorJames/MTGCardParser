namespace MTGGlyphs.GlyphDefinitions;

/// <summary>"who …" after a player: which of them it means - "who lost life this turn", "who has more cards in hand than you", "who can't".</summary>
/// <exampledoc>Teysa, Opulent Oligarch</exampledoc>
/// <examplecapture>who lost life this turn</examplecapture>
[Dependent]
public class WhoClause : Glyph
{
    public override Nib[] Nibs => ["who", Prop(Which)];

    public PlayerRelative Which { get; set; }
}
