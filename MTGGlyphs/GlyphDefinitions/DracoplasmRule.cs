namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Dracoplasm, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Dracoplasm</exampledoc>
[TokenizationOrder(-1)]
public class DracoplasmRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this creature's power becomes the total power of those creatures and its toughness becomes their total toughness"];
}
