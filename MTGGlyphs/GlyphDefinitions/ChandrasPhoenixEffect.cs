namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Chandra's Phoenix, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Chandra's Phoenix</exampledoc>
/// <examplecapture>return this card from your graveyard to your hand</examplecapture>
[Dependent]
public class ChandrasPhoenixEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["return this card from your graveyard to your hand"];
}
