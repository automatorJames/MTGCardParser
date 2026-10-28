namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Oubliette, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Oubliette</exampledoc>
[Dependent]
public class OublietteEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["auras and equipment phase out with it"];
}
