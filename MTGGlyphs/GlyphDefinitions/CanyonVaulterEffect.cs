namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Canyon Vaulter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Canyon Vaulter</exampledoc>
[Dependent]
public class CanyonVaulterEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that mount or vehicle gains flying until end of turn"];
}
