namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Bloodletter of Aclazotz, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Bloodletter of Aclazotz</exampledoc>
[TokenizationOrder(-1)]
public class BloodletterOfAclazotzRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if an opponent would lose life during your turn, they lose twice that much life instead"];
}
