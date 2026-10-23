namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Tamanoa, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Tamanoa</exampledoc>
[Dependent]
public class TamanoaEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain that much life"];
}
