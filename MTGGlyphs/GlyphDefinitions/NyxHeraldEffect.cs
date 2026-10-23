namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Nyx Herald, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nyx Herald</exampledoc>
[Dependent]
public class NyxHeraldEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target enchanted creature or enchantment creature you control gets +1/+1 and gains trample until end of turn"];
}
