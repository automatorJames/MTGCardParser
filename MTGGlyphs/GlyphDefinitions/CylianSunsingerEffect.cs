namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Cylian Sunsinger, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Cylian Sunsinger</exampledoc>
[Dependent]
public class CylianSunsingerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature and each other creature with the same name as it get +3/+3 until end of turn"];
}
