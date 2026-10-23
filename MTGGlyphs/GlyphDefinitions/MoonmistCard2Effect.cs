namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Moonmist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Moonmist</exampledoc>
[Dependent]
public class MoonmistCard2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["only double-faced cards can be transformed"];
}
