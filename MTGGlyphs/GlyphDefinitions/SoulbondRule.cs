namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Soulbond's reminder text.</summary>
/// <exampledoc>Deadeye Navigator</exampledoc>
/// <examplecapture>you may pair this creature with another unpaired creature when either enters the battlefield</examplecapture>
public class SoulbondRule : Glyph
{
    public override Nib[] Nibs => ["you may pair this creature with another unpaired creature when either enters the battlefield"];
}
