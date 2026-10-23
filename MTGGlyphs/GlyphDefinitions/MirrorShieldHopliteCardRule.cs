namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mirror-Shield Hoplite, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mirror-Shield Hoplite</exampledoc>
[TokenizationOrder(-1)]
public class MirrorShieldHopliteCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a creature you control becomes the target of a backup ability, copy that ability"];
}
