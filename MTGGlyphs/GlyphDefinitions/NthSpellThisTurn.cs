namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A spell picked out by how many the player has cast in some period, optionally of a color or type: "your second spell each turn", "your first spell each turn", "your first spell during each opponent's turn", "your third spell in a turn", "your first instant or sorcery spell each turn".</summary>
/// <exampledoc>Jori En, Ruin Diver</exampledoc>
/// <examplecapture>your second spell each turn</examplecapture>
[Dependent]
public class NthSpellThisTurn : Glyph
{
    public override Nib[] Nibs => ["your", Prop(Ordinal), Prop(Qualifiers), Prop(Or), "spell", Prop(Period)];

    public SpellOrdinal Ordinal { get; set; }
    [Optional]
    public CompoundOf<TargetQualifier> Qualifiers { get; set; }
    [Optional]
    public OrQualifier Or { get; set; }
    public SpellCountPeriod Period { get; set; }
}
