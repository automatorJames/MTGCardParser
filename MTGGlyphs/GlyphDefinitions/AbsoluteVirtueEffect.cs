namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Absolute Virtue, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Absolute Virtue</exampledoc>
[Dependent]
public class AbsoluteVirtueEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you can't be dealt damage, enchanted, or targeted by anything controlled by your opponents"];
}
