namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Ambush Commander, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Ambush Commander</exampledoc>
[TokenizationOrder(-1)]
public class AmbushCommanderRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["forests you control are 1/1 green elf creatures that are still lands"];
}
