namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Wakeroot Elemental, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Wakeroot Elemental</exampledoc>
[Dependent]
public class WakerootElementalEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this effect lasts as long as that land remains on the battlefield"];
}
