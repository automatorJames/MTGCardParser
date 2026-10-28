namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Spinerock Tyrant, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Spinerock Tyrant</exampledoc>
[TokenizationOrder(-1)]
public class SpinerockTyrantRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever you cast an instant or sorcery spell with a single target, you may copy it"];
}
