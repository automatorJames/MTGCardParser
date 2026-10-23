namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Hakim, Loreweaver, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hakim, Loreweaver</exampledoc>
[Dependent]
public class HakimLoreweaverEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["destroy all auras attached to hakim"];
}
