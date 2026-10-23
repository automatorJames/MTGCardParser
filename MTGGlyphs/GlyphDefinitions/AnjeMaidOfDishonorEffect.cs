namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Anje, Maid of Dishonor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Anje, Maid of Dishonor</exampledoc>
[Dependent]
public class AnjeMaidOfDishonorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["create a blood token"];
}
