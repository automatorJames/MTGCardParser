namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Intimidate's reminder text.</summary>
/// <exampledoc>Academy Raider</exampledoc>
/// <examplecapture>this creature can't be blocked except by artifact creatures and/or creatures that share a color with it</examplecapture>
public class IntimidateRule : Glyph
{
    public override Nib[] Nibs => ["this creature can't be blocked except by artifact creatures and/or creatures that share a color with it"];
}
