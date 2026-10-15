namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Deathtouch's reminder text, worded for a granted ability ("it deals").</summary>
/// <exampledoc>Acidic Slime</exampledoc>
/// <examplecapture>any amount of damage it deals to a creature is enough to destroy it</examplecapture>
public class DeathtouchItRule : Glyph
{
    public override Nib[] Nibs => ["any amount of damage it deals to a creature is enough to destroy it"];
}
