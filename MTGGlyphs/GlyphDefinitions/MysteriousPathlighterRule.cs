namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Mysterious Pathlighter, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Mysterious Pathlighter</exampledoc>
[TokenizationOrder(-1)]
public class MysteriousPathlighterRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each creature you control that has an adventure enters with an additional +1/+1 counter on it"];
}
