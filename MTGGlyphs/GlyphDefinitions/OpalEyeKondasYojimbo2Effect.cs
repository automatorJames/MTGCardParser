namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Opal-Eye, Konda's Yojimbo, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Opal-Eye, Konda's Yojimbo</exampledoc>
[Dependent]
public class OpalEyeKondasYojimbo2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["prevent the next 1 damage that would be dealt to opal-eye this turn"];
}
