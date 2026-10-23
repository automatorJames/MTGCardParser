namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Shark Shredder, Killer Clone, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Shark Shredder, Killer Clone</exampledoc>
[TokenizationOrder(-1)]
public class SharkShredderKillerCloneRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["it enters tapped and attacking that player"];
}
