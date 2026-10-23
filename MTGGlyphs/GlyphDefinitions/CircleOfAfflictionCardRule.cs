namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Circle of Affliction, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Circle of Affliction</exampledoc>
[TokenizationOrder(-1)]
public class CircleOfAfflictionCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["whenever a source of the chosen color deals damage to you, you may pay {1}"];
}
