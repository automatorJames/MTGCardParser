namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Nightbound's reminder text.</summary>
/// <exampledoc>Tavern Ruffian // Tavern Smasher</exampledoc>
/// <examplecapture>if a player casts at least two spells during their own turn, it becomes day next turn</examplecapture>
public class NightboundRule : Glyph
{
    public override Nib[] Nibs => ["if a player casts at least two spells during their own turn, it becomes day next turn"];
}
