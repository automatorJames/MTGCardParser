namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Oath of Lim-Dûl, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Oath of Lim-Dûl</exampledoc>
[Dependent]
public class OathOfLimDulEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["damage dealt to you causes you to lose life"];
}
