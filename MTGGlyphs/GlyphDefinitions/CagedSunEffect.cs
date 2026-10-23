namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Caged Sun, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Caged Sun</exampledoc>
[Dependent]
public class CagedSunEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["add an additional one mana of that color"];
}
