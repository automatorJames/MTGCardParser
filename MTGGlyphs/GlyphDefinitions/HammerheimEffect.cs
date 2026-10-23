namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Hammerheim, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hammerheim</exampledoc>
[Dependent]
public class HammerheimEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target creature loses all landwalk abilities until end of turn"];
}
