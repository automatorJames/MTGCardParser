namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Soulbond's reminder text, first sentence (SoulbondPaired is the second).</summary>
/// <exampledoc>Deadeye Navigator</exampledoc>
/// <examplecapture>you may pair this creature with another unpaired creature when either enters</examplecapture>
public class SoulbondRule : Glyph
{
    public override Nib[] Nibs => ["you may pair this creature with another unpaired creature when either", Alt("enters", "enters the battlefield")];
}
