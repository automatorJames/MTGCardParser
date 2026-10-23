namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Hiveheart Shaman, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Hiveheart Shaman</exampledoc>
/// <examplecapture>you may search your library for a basic land card that doesn't share a land type with a land you control, put that card onto the battlefield, then shuffle</examplecapture>
[Dependent]
public class HiveheartShamanEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["you may search your library for a basic land card that doesn't share a land type with a land you control, put that card onto the battlefield, then shuffle"];
}
