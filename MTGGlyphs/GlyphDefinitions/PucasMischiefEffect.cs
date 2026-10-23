namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Puca's Mischief, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Puca's Mischief</exampledoc>
/// <examplecapture>you may exchange control of target nonland permanent you control and target nonland permanent an opponent controls with equal or lesser mana value</examplecapture>
[Dependent]
public class PucasMischiefEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may exchange control of target nonland permanent you control and target nonland permanent an opponent controls with equal or lesser mana value"];
}
