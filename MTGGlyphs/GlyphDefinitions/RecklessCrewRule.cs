namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Reckless Crew, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Reckless Crew</exampledoc>
[TokenizationOrder(-1)]
public class RecklessCrewRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["for each of those tokens, you may attach an equipment you control to it"];
}
