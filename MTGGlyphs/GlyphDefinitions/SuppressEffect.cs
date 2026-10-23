namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Suppress, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Suppress</exampledoc>
[Dependent]
public class SuppressEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["that player returns those cards to their hand"];
}
