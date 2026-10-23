namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Zendikar Resurgent, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Zendikar Resurgent</exampledoc>
[Dependent]
public class ZendikarResurgentEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the types of mana are white, blue, black, red, green, and colorless"];
}
