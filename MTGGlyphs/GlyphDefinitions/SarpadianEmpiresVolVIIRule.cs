namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific rules text of Sarpadian Empires, Vol. VII, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Sarpadian Empires, Vol. VII</exampledoc>
[TokenizationOrder(-1)]
public class SarpadianEmpiresVolVIIRule : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["as this artifact enters, choose white citizen, blue camarid, black thrull, red goblin, or green saproling"];
}
