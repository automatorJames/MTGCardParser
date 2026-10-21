namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Deathtouch's reminder text, for the card itself, a granted ability or a group: "any amount of damage this deals to a creature is enough to destroy it", "… it deals …", "… they deal …", "… a creature with deathtouch deals … is enough to destroy that creature".</summary>
/// <exampledoc>Acidic Slime</exampledoc>
/// <examplecapture>any amount of damage this deals to a creature is enough to destroy it</examplecapture>
public class DeathtouchRule : Glyph
{
    public override Nib[] Nibs => ["any amount of damage", Prop(Source), Pattern("deals?"), "to a creature is enough to destroy", Pattern("(it|that creature)")];

    public Subject Source { get; set; }
}
