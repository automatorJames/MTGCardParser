namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Runed Halo, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Runed Halo</exampledoc>
[Dependent]
public class RunedHaloEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you can't be targeted, dealt damage, or enchanted by anything with that name"];
}
