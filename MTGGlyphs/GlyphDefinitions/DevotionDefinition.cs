namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Devotion's general reminder, for cards that ask for devotion to a chosen color: "your devotion to a color is the number of mana symbols of that color in the mana costs of permanents you control".</summary>
/// <exampledoc>Nykthos, Shrine to Nyx</exampledoc>
/// <examplecapture>(your devotion to a color is the number of mana symbols of that color in the mana costs of permanents you control</examplecapture>
public class DevotionDefinition : Glyph
{
    public override Nib[] Nibs => ["(your devotion to a color is the number of mana symbols of that color in the mana costs of permanents you control"];
}
