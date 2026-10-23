namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Stolen Uniform, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Stolen Uniform</exampledoc>
[TokenizationOrder(-1)]
public class StolenUniformCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when you lose control of that equipment this turn, if it's attached to a creature you control, unattach it"];
}
