namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Novijen Sages, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Novijen Sages</exampledoc>
[Dependent]
public class NovijenSagesEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["draw a card"];
}
