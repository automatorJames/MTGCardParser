namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Firemind's Foresight, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Firemind's Foresight</exampledoc>
[TokenizationOrder(-1)]
public class FiremindsForesightRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["then repeat this process for instant cards with mana values 2 and 1"];
}
