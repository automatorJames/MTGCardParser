namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Jace's Archivist, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Jace's Archivist</exampledoc>
[Dependent]
public class JacesArchivistEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each player discards their hand, then draws cards equal to the greatest number of cards a player discarded this way"];
}
