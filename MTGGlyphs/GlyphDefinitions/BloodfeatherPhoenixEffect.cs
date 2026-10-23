namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Bloodfeather Phoenix, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bloodfeather Phoenix</exampledoc>
[Dependent]
public class BloodfeatherPhoenixEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may pay {r}"];
}
