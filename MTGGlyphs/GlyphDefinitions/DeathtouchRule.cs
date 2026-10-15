namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Deathtouch's reminder text: "any amount of damage this deals to a creature is enough to destroy it".</summary>
/// <exampledoc>Acidic Slime</exampledoc>
/// <examplecapture>any amount of damage this deals to a creature is enough to destroy it</examplecapture>
public class DeathtouchRule : Glyph
{
    public override Nib[] Nibs => ["any amount of damage this deals to a creature is enough to destroy it"];
}
