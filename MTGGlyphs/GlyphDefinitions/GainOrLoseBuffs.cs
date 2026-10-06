namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Several buffs gained or lost together.</summary>
public class GainOrLoseBuffs : Glyph
{
    public ManyOf<Buff> GainedOrLostBuffs { get; set; }
}