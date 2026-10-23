namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Umbra Mystic, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Umbra Mystic</exampledoc>
[Dependent]
public class UmbraMysticEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if an enchanted permanent you control would be destroyed, instead remove all damage from it and destroy an aura attached to it"];
}
