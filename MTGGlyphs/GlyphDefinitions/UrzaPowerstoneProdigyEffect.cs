namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Urza, Powerstone Prodigy, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Urza, Powerstone Prodigy</exampledoc>
[Dependent]
public class UrzaPowerstoneProdigyEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["the token is an artifact with \"{t}: add {c}"];
}
