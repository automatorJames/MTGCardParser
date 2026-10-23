namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Magma Pummeler, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Magma Pummeler</exampledoc>
[Dependent]
public class MagmaPummelerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it deals that much damage to any target"];
}
