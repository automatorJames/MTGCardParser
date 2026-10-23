namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Verity Circle, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Verity Circle</exampledoc>
[Dependent]
public class VerityCircleEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if it isn't being declared as an attacker, you may draw a card"];
}
