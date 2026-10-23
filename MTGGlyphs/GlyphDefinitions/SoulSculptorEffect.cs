namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Soul Sculptor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Soul Sculptor</exampledoc>
[Dependent]
public class SoulSculptorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["target creature becomes an enchantment and loses all abilities until a player casts a creature spell"];
}
