namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Falkenrath Gorger, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Falkenrath Gorger</exampledoc>
[Dependent]
public class FalkenrathGorger2Effect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["if you discard a card with madness, discard it into exile"];
}
