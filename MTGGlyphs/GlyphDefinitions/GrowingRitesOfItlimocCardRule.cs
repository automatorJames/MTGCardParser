namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Growing Rites of Itlimoc // Itlimoc, Cradle of the Sun, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Growing Rites of Itlimoc // Itlimoc, Cradle of the Sun</exampledoc>
[TokenizationOrder(-1)]
public class GrowingRitesOfItlimocCardRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["when growing rites of itlimoc enters, look at the top four cards of your library"];
}
