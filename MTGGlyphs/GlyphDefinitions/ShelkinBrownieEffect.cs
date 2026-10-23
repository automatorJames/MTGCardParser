namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Shelkin Brownie, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shelkin Brownie</exampledoc>
[Dependent]
public class ShelkinBrownieEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target creature loses all \"bands with other\" abilities until end of turn"];
}
