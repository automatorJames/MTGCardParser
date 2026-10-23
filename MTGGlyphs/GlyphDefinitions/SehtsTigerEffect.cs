namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Seht's Tiger, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Seht's Tiger</exampledoc>
[Dependent]
public class SehtsTigerEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you can't be targeted, dealt damage, or enchanted by anything of the chosen color"];
}
