namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Hydroid Krasis, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hydroid Krasis</exampledoc>
[Dependent]
public class HydroidKrasisEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain half x life and draw half x cards"];
}
