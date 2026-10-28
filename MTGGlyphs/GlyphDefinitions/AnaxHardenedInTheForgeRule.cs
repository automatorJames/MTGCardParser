namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Anax, Hardened in the Forge, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Anax, Hardened in the Forge</exampledoc>
[TokenizationOrder(-1)]
public class AnaxHardenedInTheForgeRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if the creature had power 4 or greater, create two of those tokens instead"];
}
