namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Summon: Brynhildr, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Summon: Brynhildr</exampledoc>
[TokenizationOrder(-1)]
public class SummonBrynhildrRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["during any turn you put a lore counter on this saga, you may play that card"];
}
