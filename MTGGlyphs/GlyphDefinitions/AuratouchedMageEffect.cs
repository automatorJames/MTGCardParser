namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Auratouched Mage, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Auratouched Mage</exampledoc>
[Dependent]
public class AuratouchedMageEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["search your library for an aura card that could enchant it"];
}
