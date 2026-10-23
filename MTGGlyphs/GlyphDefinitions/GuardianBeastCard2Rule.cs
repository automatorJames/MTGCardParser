namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Guardian Beast, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Guardian Beast</exampledoc>
[TokenizationOrder(-1)]
public class GuardianBeastCard2Rule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["this effect doesn't remove auras already attached to those artifacts"];
}
