namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Quintorius, Loremaster, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Quintorius, Loremaster</exampledoc>
[Dependent]
public class QuintoriusLoremasterEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["choose target card exiled with quintorius"];
}
