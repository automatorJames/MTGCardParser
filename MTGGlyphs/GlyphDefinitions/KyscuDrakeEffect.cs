namespace MTGGlyphs.GlyphDefinitions;

/// <summary>Card-specific effect of Kyscu Drake, held as one literal for the game engine until the grammar models it.</summary>
/// <exampledoc>Kyscu Drake</exampledoc>
/// <examplecapture>search your library for a card named viashivan dragon, put that card onto the battlefield, then shuffle</examplecapture>
[Dependent]
public class KyscuDrakeEffect : Glyph, ICardSpecific
{
    public override Nib[] Nibs => ["search your library for a card named viashivan dragon, put that card onto the battlefield, then shuffle"];
}
