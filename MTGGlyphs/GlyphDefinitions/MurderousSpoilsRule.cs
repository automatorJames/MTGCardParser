namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Murderous Spoils, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Murderous Spoils</exampledoc>
[TokenizationOrder(-1)]
public class MurderousSpoilsRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you gain control of all equipment that were attached to it"];
}
