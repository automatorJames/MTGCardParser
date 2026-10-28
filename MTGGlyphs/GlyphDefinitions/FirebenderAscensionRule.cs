namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Firebender Ascension, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Firebender Ascension</exampledoc>
[TokenizationOrder(-1)]
public class FirebenderAscensionRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a creature you control attacking causes a triggered ability of that creature to trigger, put a quest counter on this enchantment"];
}
