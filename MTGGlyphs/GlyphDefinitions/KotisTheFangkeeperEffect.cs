namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Kotis, the Fangkeeper, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kotis, the Fangkeeper</exampledoc>
[Dependent]
public class KotisTheFangkeeperEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["exile the top x cards of their library, where x is the amount of damage dealt"];
}
