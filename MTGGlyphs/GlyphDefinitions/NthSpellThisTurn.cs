namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A spell picked out by how many the player has cast this turn: "your second spell each turn", "your first spell each turn".</summary>
/// <exampledoc>Jori En, Ruin Diver</exampledoc>
/// <examplecapture>your second spell each turn</examplecapture>
[Dependent]
public class NthSpellThisTurn : Glyph
{
    public override Nib[] Nibs => ["your", Prop(Ordinal), "spell each turn"];

    public SpellOrdinal Ordinal { get; set; }
}
