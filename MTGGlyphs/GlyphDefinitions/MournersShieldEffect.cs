namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Mourner's Shield, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mourner's Shield</exampledoc>
[Dependent]
public class MournersShieldEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["prevent all damage that would be dealt this turn by a source of your choice that shares a color with the exiled card"];
}
