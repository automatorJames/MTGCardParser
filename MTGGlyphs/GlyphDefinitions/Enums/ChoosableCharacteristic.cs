namespace MTGGlyphs.GlyphDefinitions;

/// <summary>A characteristic the controller picks as an effect applies, between "the" and "of your choice": "the color of your choice", "the color or colors of your choice", "the creature type of your choice", "the basic land type of your choice".</summary>
public enum ChoosableCharacteristic
{
    [RegexPattern("color or colors")]
    ColorOrColors,

    Color,

    CreatureType,

    BasicLandType
}
