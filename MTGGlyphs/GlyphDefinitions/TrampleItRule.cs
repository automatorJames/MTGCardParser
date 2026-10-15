namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Excess-damage trample reminder text.</summary>
/// <exampledoc>Thundering Giant</exampledoc>
/// <examplecapture>it can deal excess combat damage to the player or planeswalker it's attacking</examplecapture>
public class TrampleItRule : Glyph
{
    public override Nib[] Nibs => ["it can deal excess combat damage to the player or planeswalker it's attacking"];
}
