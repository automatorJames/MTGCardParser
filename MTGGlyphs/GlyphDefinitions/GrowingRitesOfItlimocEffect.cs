namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Growing Rites of Itlimoc // Itlimoc, Cradle of the Sun, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Growing Rites of Itlimoc // Itlimoc, Cradle of the Sun</exampledoc>
[Dependent]
public class GrowingRitesOfItlimocEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["look at the top four cards of your library"];
}
