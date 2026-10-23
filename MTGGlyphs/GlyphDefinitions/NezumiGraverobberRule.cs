namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Nezumi Graverobber // Nighteyes the Desecrator, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Nezumi Graverobber // Nighteyes the Desecrator</exampledoc>
[TokenizationOrder(-1)]
public class NezumiGraverobberRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if no cards are in that graveyard, flip this creature"];
}
