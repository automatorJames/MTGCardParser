namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Evoke's reminder consequence: a spell cast for its evoke cost is sacrificed as it enters.</summary>
/// <exampledoc>Dawnfluke</exampledoc>
/// <examplecapture>if you do, it's sacrificed when it enters</examplecapture>
public class EvokeSacrificeRule : Glyph
{
    public override Nib[] Nibs => ["if you do, it's sacrificed when it", Alt("enters", "enters the battlefield")];
}
