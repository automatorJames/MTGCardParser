namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Lightstall Inquisitor, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Lightstall Inquisitor</exampledoc>
[Dependent]
public class LightstallInquisitorEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["each opponent exiles a card from their hand and may play that card for as long as it remains exiled"];
}
