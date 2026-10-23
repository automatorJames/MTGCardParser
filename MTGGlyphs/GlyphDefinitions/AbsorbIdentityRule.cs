namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Absorb Identity, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Absorb Identity</exampledoc>
[TokenizationOrder(-1)]
public class AbsorbIdentityRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may have shapeshifters you control become copies of that creature until end of turn"];
}
