namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Kaya, Orzhov Usurper, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kaya, Orzhov Usurper</exampledoc>
[Dependent]
public class KayaOrzhovUsurperEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["kaya deals damage to target player equal to the number of cards that player owns in exile and you gain that much life"];
}
