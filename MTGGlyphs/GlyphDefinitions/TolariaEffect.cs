namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Tolaria, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tolaria</exampledoc>
[Dependent]
public class TolariaEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target creature loses banding and all \"bands with other\" abilities until end of turn"];
}
