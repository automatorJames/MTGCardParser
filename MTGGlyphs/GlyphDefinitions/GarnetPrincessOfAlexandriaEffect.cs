namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Garnet, Princess of Alexandria, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Garnet, Princess of Alexandria</exampledoc>
[Dependent]
public class GarnetPrincessOfAlexandriaEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may remove a lore counter from each of any number of sagas you control"];
}
