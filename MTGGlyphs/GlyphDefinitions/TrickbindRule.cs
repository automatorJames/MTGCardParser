namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Trickbind, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Trickbind</exampledoc>
[TokenizationOrder(-1)]
public class TrickbindRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if a permanent's ability is countered this way, activated abilities of that permanent can't be activated this turn"];
}
