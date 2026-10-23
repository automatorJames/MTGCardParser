namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Urza's Miter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Urza's Miter</exampledoc>
[Dependent]
public class UrzasMiterEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it wasn't sacrificed, you may pay {3}"];
}
